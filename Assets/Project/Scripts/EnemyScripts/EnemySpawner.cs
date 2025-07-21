using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public int enemyCount = 3;
    public float spawnRadius = 2f;
    public bool spawnOnStart = true;

    [Header("Debug")]
    public bool showDebugInfo = true;

    private int enemiesSpawned = 0;

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnEnemies();
        }
    }

    [ContextMenu("Spawn Enemies")]
    public void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: Nenhum prefab de Enemy definido!");
            return;
        }

        if (showDebugInfo)
        {
            Debug.Log($"Spawner '{gameObject.name}' iniciando spawn de {enemyCount} enemies...");
        }

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnSingleEnemy();
        }
    }

    void SpawnSingleEnemy()
    {
        // Encontrar posição válida para spawn
        Vector3 spawnPosition = FindValidSpawnPosition();

        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning($"EnemySpawner: Não foi possível encontrar posição válida para spawn #{enemiesSpawned + 1}");
            return;
        }

        // Instanciar Enemy
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        newEnemy.name = $"Enemy_Spawned_{enemiesSpawned + 1}";

        // 🎯 CONFIGURAR PATROL POINTS AUTOMATICAMENTE
        ConfigureEnemyPatrolPoints(newEnemy);

        enemiesSpawned++;

        if (showDebugInfo)
        {
            Debug.Log($"Enemy #{enemiesSpawned} spawnado em {spawnPosition}");
        }
    }

    void ConfigureEnemyPatrolPoints(GameObject enemy)
    {
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI == null)
        {
            Debug.LogError($"Enemy {enemy.name} não possui EnemyAI component!");
            return;
        }

        // Buscar todos os patrol points na cena
        GameObject[] patrolObjects = GameObject.FindGameObjectsWithTag("PatrolPoint");

        if (patrolObjects.Length == 0)
        {
            Debug.LogWarning("Nenhum PatrolPoint encontrado na cena! Verifique as tags.");
            return;
        }

        // Converter para Transform array
        Transform[] patrolPoints = new Transform[patrolObjects.Length];
        for (int i = 0; i < patrolObjects.Length; i++)
        {
            patrolPoints[i] = patrolObjects[i].transform;
        }

        // Configurar o enemy usando reflexão para acessar campo privado
        System.Reflection.FieldInfo patrolField = typeof(EnemyAI).GetField("patrolPoints",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (patrolField != null)
        {
            patrolField.SetValue(enemyAI, patrolPoints);
            Debug.Log($"Enemy {enemy.name} configurado com {patrolPoints.Length} patrol points");
        }
        else
        {
            Debug.LogError("Não foi possível acessar campo patrolPoints no EnemyAI!");
        }
    }

    Vector3 FindValidSpawnPosition()
    {
        int maxAttempts = 20;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Gerar posição aleatória ao redor do spawner
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 candidatePosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Verificar se a posição está no NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePosition, out hit, 1f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // Se não encontrou posição válida, usar posição do spawner
        NavMeshHit spawnerHit;
        if (NavMesh.SamplePosition(transform.position, out spawnerHit, 5f, NavMesh.AllAreas))
        {
            return spawnerHit.position;
        }

        return Vector3.zero; // Falha total
    }

    // Visualização no Scene View
    void OnDrawGizmosSelected()
    {
        // Raio de spawn
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        // Indicador do spawner
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, Vector3.one);
    }

    // Interface de debug
    void OnGUI()
    {
        if (showDebugInfo && Application.isPlaying)
        {
            GUI.Box(new Rect(Screen.width - 220, 10, 200, 60), "");
            GUI.Label(new Rect(Screen.width - 215, 15, 190, 20), $"Spawner: {gameObject.name}");
            GUI.Label(new Rect(Screen.width - 215, 35, 190, 20), $"Enemies spawnados: {enemiesSpawned}");

            if (GUI.Button(new Rect(Screen.width - 215, 50, 100, 20), "Spawn Mais"))
            {
                SpawnSingleEnemy();
            }
        }
    }
}