using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTime = 2f;

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    public float fieldOfViewAngle = 60f;
    public LayerMask obstacleLayerMask = -1;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackDuration = 0.3f;

    [Header("Debug Visualization")]
    public bool showDebugRays = true;
    public bool showDetectionInfo = true;

    private NavMeshAgent agent;
    private Transform player;
    private Renderer enemyRenderer;
    private Color originalColor;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private Vector3 lastKnownPlayerPosition;
    private bool playerDetected = false;
    private float lastAttackTime = 0f;
    private bool isAttacking = false;

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
        enemyRenderer = GetComponent<Renderer>();

        // Guardar cor original
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }

        // Começar patrulhando
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    void Update()
    {
        // Verificar detecção do player
        playerDetected = CanSeePlayer();
        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.position) : float.MaxValue;

        // Lógica de transição de estados
        switch (currentState)
        {
            case EnemyState.PATROL:
                if (playerDetected)
                {
                    lastKnownPlayerPosition = player.position;
                    ChangeState(EnemyState.CHASE);
                }
                else
                {
                    PatrolBehavior();
                }
                break;

            case EnemyState.CHASE:
                if (!playerDetected)
                {
                    ChangeState(EnemyState.PATROL);
                }
                else if (distanceToPlayer <= attackRange)
                {
                    ChangeState(EnemyState.ATTACK);
                }
                else
                {
                    ChaseBehavior();
                }
                break;

            case EnemyState.ATTACK:
                // SÓ sai do estado ATTACK se:
                // 1. Não vê mais o player OU
                // 2. Player está muito longe
                if (!playerDetected || distanceToPlayer > attackRange * 1.5f)
                {
                    ChangeState(EnemyState.CHASE);
                }
                else
                {
                    // CONTINUA atacando enquanto player estiver no alcance
                    AttackBehavior();
                }
                break;

            case EnemyState.SEARCH:
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
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void AttackBehavior()
    {
        // Parar movimento e olhar para o player
        agent.isStopped = true;

        // Rotacionar para o player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // Manter rotação apenas no eixo Y
        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);
        }

        // Verificar se pode atacar (cooldown)
        if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // PASSO 1.2: Feedback Visual (piscar amarelo)
        StartCoroutine(AttackVisualFeedback());

        // Debug do ataque
        if (showDetectionInfo)
        {
            Debug.Log($"Enemy atacou o Player! Dano aplicado.");
        }

        // TODO: Aplicar dano no player aqui (Passo 1.3)

        // Aguardar duração do ataque
        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
    }

    IEnumerator AttackVisualFeedback()
    {
        if (enemyRenderer == null) yield break;

        // Piscar para amarelo
        enemyRenderer.material.color = Color.yellow;
        yield return new WaitForSeconds(attackDuration * 0.5f);

        // Voltar para cor original
        enemyRenderer.material.color = originalColor;
        yield return new WaitForSeconds(attackDuration * 0.5f);

        // Piscar novamente para dar ênfase
        enemyRenderer.material.color = Color.yellow;
        yield return new WaitForSeconds(attackDuration * 0.25f);

        // Cor final
        enemyRenderer.material.color = originalColor;
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        // ETAPA 1: Verificar distância
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRadius) return false;

        // ETAPA 2: Campo de visão adaptativo baseado na distância
        float adaptiveFieldOfView = fieldOfViewAngle;

        // Quando mais próximo, campo de visão aumenta
        if (distanceToPlayer <= attackRange * 2f) // Zona próxima
        {
            // Interpolar entre fieldOfViewAngle normal e 180°
            float proximityFactor = 1f - (distanceToPlayer / (attackRange * 2f));
            adaptiveFieldOfView = Mathf.Lerp(fieldOfViewAngle, 180f, proximityFactor);
        }

        // Durante ATTACK, campo de visão ainda mais amplo
        if (currentState == EnemyState.ATTACK)
        {
            adaptiveFieldOfView = 180f; // Quase impossível "escapar" durante ataque
        }

        // Verificar ângulo de visão com campo adaptativo
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > adaptiveFieldOfView / 2) return false;

        // ETAPA 3: Verificar linha de visão (raycast para obstáculos)
        Vector3 rayStart = transform.position + Vector3.up * 1.5f;
        Ray ray = new Ray(rayStart, directionToPlayer);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRadius, obstacleLayerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
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

        // Reset do agent quando necessário
        if (newState == EnemyState.CHASE || newState == EnemyState.PATROL)
        {
            agent.isStopped = false;
        }
    }

    void DrawDebugRays()
    {
        // CONE DE VISÃO (linhas laterais)
        Vector3 leftBoundary = Quaternion.AngleAxis(-fieldOfViewAngle / 2, Vector3.up) * transform.forward * detectionRadius;
        Vector3 rightBoundary = Quaternion.AngleAxis(fieldOfViewAngle / 2, Vector3.up) * transform.forward * detectionRadius;

        Debug.DrawLine(transform.position + Vector3.up * 1.5f, transform.position + leftBoundary + Vector3.up * 1.5f, Color.yellow);
        Debug.DrawLine(transform.position + Vector3.up * 1.5f, transform.position + rightBoundary + Vector3.up * 1.5f, Color.yellow);
        Debug.DrawLine(transform.position + leftBoundary + Vector3.up * 1.5f, transform.position + rightBoundary + Vector3.up * 1.5f, Color.yellow);

        // CÍRCULO DE ATAQUE (vermelho)
        for (int i = 0; i < 24; i++)
        {
            float angle1 = i * 15f * Mathf.Deg2Rad;
            float angle2 = (i + 1) * 15f * Mathf.Deg2Rad;

            Vector3 point1 = transform.position + new Vector3(Mathf.Sin(angle1), 0, Mathf.Cos(angle1)) * attackRange;
            Vector3 point2 = transform.position + new Vector3(Mathf.Sin(angle2), 0, Mathf.Cos(angle2)) * attackRange;

            Debug.DrawLine(point1, point2, Color.red, 0.1f);
        }

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
        // Raio de detecção
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Raio de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

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
            GUI.Box(new Rect(10, 10, 200, 120), "");
            GUI.Label(new Rect(15, 15, 190, 20), $"Estado: {currentState}");
            GUI.Label(new Rect(15, 35, 190, 20), $"Player detectado: {playerDetected}");
            GUI.Label(new Rect(15, 55, 190, 20), $"Atacando: {isAttacking}");

            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                GUI.Label(new Rect(15, 75, 190, 20), $"Distância: {distance:F1}m");
                GUI.Label(new Rect(15, 95, 190, 20), $"No alcance: {distance <= attackRange}");
            }
        }
    }
}