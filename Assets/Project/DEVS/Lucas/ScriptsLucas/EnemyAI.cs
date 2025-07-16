using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTime = 2f;

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float fieldOfViewAngle = 60f;
    public LayerMask obstacleLayerMask = -1;

    [Header("Debug Visualization")]
    public bool showDebugRays = true;
    public bool showDetectionInfo = true;

    private NavMeshAgent agent;
    private Transform player;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Vector3 lastKnownPlayerPosition;
    private bool playerDetected = false;

    // Estados da FSM
    public enum EnemyState
    {
        PATROL,
        CHASE,
        SEARCH,
        ATTACK
    }

    public EnemyState currentState = EnemyState.PATROL;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Começar patrulhando
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    void Update()
    {
        // Verificar detecção do player (exceto quando atacando)
        if (currentState != EnemyState.ATTACK)
        {
            playerDetected = CanSeePlayer();
            if (playerDetected)
            {
                lastKnownPlayerPosition = player.position;
                ChangeState(EnemyState.CHASE);
            }
        }

        // Executar comportamento do estado atual
        switch (currentState)
        {
            case EnemyState.PATROL:
                PatrolBehavior();
                break;
            case EnemyState.CHASE:
                ChaseBehavior();
                break;
            case EnemyState.SEARCH:
                // Implementaremos depois
                break;
            case EnemyState.ATTACK:
                // Implementaremos depois
                break;
        }

        // Debug visual em tempo real
        if (showDebugRays)
        {
            DrawDebugRays();
        }
    }

    void PatrolBehavior()
    {
        // Se chegou no destino
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = waitTime;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    // Ir para próximo ponto
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                    isWaiting = false;
                }
            }
        }
    }

    void ChaseBehavior()
    {
        // Perseguir o player
        agent.SetDestination(player.position);

        // Se perdeu o player, voltar para patrulha
        if (!CanSeePlayer())
        {
            ChangeState(EnemyState.PATROL);
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        // ETAPA 1: Verificar distância
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRadius) return false;

        // ETAPA 2: Verificar ângulo de visão (campo de visão)
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfViewAngle / 2) return false;

        // ETAPA 3: Verificar linha de visão (raycast para obstáculos)
        Vector3 rayStart = transform.position + Vector3.up * 1.5f; // Altura dos "olhos"
        Ray ray = new Ray(rayStart, directionToPlayer);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRadius, obstacleLayerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true; // Vê o player!
            }
        }

        return false; // Algo está bloqueando a visão
    }

    void ChangeState(EnemyState newState)
    {
        if (showDetectionInfo)
        {
            Debug.Log($"Enemy mudou estado: {currentState} → {newState}");
        }

        currentState = newState;

        // Reset do waiting quando muda estado
        isWaiting = false;
        waitTimer = 0f;
    }

    void DrawDebugRays()
    {
        // CONE DE VISÃO (linhas laterais)
        Vector3 leftBoundary = Quaternion.AngleAxis(-fieldOfViewAngle / 2, Vector3.up) * transform.forward * detectionRadius;
        Vector3 rightBoundary = Quaternion.AngleAxis(fieldOfViewAngle / 2, Vector3.up) * transform.forward * detectionRadius;

        Debug.DrawLine(transform.position + Vector3.up * 1.5f, transform.position + leftBoundary + Vector3.up * 1.5f, Color.yellow);
        Debug.DrawLine(transform.position + Vector3.up * 1.5f, transform.position + rightBoundary + Vector3.up * 1.5f, Color.yellow);
        Debug.DrawLine(transform.position + leftBoundary + Vector3.up * 1.5f, transform.position + rightBoundary + Vector3.up * 1.5f, Color.yellow);

        // RAYCAST PARA O PLAYER (se dentro do campo de visão)
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (distanceToPlayer <= detectionRadius && angle <= fieldOfViewAngle / 2)
            {
                // Raycast para verificar obstáculos
                Color rayColor = playerDetected ? Color.green : Color.red;
                Debug.DrawLine(transform.position + Vector3.up * 1.5f, player.position + Vector3.up * 1f, rayColor, 0.1f);
            }
        }

        // CÍRCULO DE DETECÇÃO (aproximado com linhas)
        for (int i = 0; i < 36; i++)
        {
            float angle1 = i * 10f * Mathf.Deg2Rad;
            float angle2 = (i + 1) * 10f * Mathf.Deg2Rad;

            Vector3 point1 = transform.position + new Vector3(Mathf.Sin(angle1), 0, Mathf.Cos(angle1)) * detectionRadius;
            Vector3 point2 = transform.position + new Vector3(Mathf.Sin(angle2), 0, Mathf.Cos(angle2)) * detectionRadius;

            Debug.DrawLine(point1, point2, Color.cyan, 0.1f);
        }
    }

    // Visualizar no Scene View (quando selecionar o objeto)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Mostrar campo de visão
        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.AngleAxis(-fieldOfViewAngle / 2, Vector3.up) * transform.forward * detectionRadius;
        Vector3 rightBoundary = Quaternion.AngleAxis(fieldOfViewAngle / 2, Vector3.up) * transform.forward * detectionRadius;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }

    // Interface para debug no Inspector
    void OnGUI()
    {
        if (showDetectionInfo && Application.isPlaying)
        {
            GUI.Box(new Rect(10, 10, 200, 100), "");
            GUI.Label(new Rect(15, 15, 190, 20), $"Estado: {currentState}");
            GUI.Label(new Rect(15, 35, 190, 20), $"Player detectado: {playerDetected}");

            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                GUI.Label(new Rect(15, 55, 190, 20), $"Distância: {distance:F1}m");

                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);
                GUI.Label(new Rect(15, 75, 190, 20), $"Ângulo: {angle:F1}°");
            }
        }
    }
}