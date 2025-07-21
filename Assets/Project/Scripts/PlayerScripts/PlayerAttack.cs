using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameController gameController;
    public float katanaAttackRange = 1.5f;
    private double katanaAttackDamage;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public Animator katanaAnimator;

    [Header("Cooldown")]
    public float attackRate = 2f; // ataques por segundo
    private float nextAttackTime = 0f;

    // Combo variables
    private int comboStep = 0;
    private float lastAttackTime;
    public float comboResetTime = 1f;

    // FMOD variables
    private EventInstance ataque;
    private EventInstance ataque_no_inimigo;

    // Novo: controla se combo está ativo
    private bool IsComboActive = false;

    void Start()
    {
        // FMOD
        ataque = RuntimeManager.CreateInstance("event:/Player_Ataca_Espada");
        ataque_no_inimigo = RuntimeManager.CreateInstance("event:/Player_AtacaAtinge_Espada");
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        // Se o combo está ativo e passou do tempo para resetar, desativa o combo
        if (IsComboActive && Time.time - lastAttackTime > comboResetTime)
        {
            IsComboActive = false;
            katanaAnimator.SetBool("IsComboActive", false);  // Diz pro animator que combo acabou
            comboStep = 0; // Reinicia o combo
            Debug.Log("Combo resetado por timeout");
        }

        if (Time.unscaledTime >= nextAttackTime && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
            nextAttackTime = Time.unscaledTime + 1f / attackRate;
        }
    }

    void Attack()
    {
        Debug.Log("ATACOU!");
        katanaAttackDamage = gameController.getKatanaDamage();

        // Detectar inimigos no alcance
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, katanaAttackRange, enemyLayers);

        bool atingiuAlvo = false;

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Acertou " + enemy.name + " e deu " + katanaAttackDamage + " de dano");
            enemy.GetComponentInParent<EnemyAI>()?.enemyTakeDamage(katanaAttackDamage);
            atingiuAlvo = true;
            gameController.addPlayerPoints(10);
        }

        if (atingiuAlvo)
        {
            ataque_no_inimigo.start(); // Se atingiu algum inimigo
        }
        else
        {
            ataque.start(); // Se não atingiu ninguém
        }

        // Se chegou aqui, combo está ativo (clicou dentro do tempo)
        IsComboActive = true;
        katanaAnimator.SetBool("IsComboActive", true);

        lastAttackTime = Time.time;

        katanaAnimator.SetInteger("ComboIndex", comboStep);
        katanaAnimator.SetTrigger("DoCombo");
        Debug.Log("ComboIndex atual: " + comboStep);

        comboStep = (comboStep + 1) % 4; // Vai de 0 até 4 (ajuste conforme suas animações)
    }

    // Gizmo pra visualizar o alcance do ataque no editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, katanaAttackRange);
    }

    void OnDestroy()
    {
        ataque.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ataque.release();

        ataque_no_inimigo.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ataque_no_inimigo.release();
    }
}
