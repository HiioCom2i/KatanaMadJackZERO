using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public int attackDamage = 1;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Cooldown")]
    public float attackRate = 2f; // ataques por segundo
    private float nextAttackTime = 0f;

    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetButtonDown("Fire1"))
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    void Attack()
    {
        Debug.Log("ATACOU!");

        // Detectar inimigos no alcance
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Acertou " + enemy.name);
            // Aqui você pode acessar o script do inimigo e causar dano, tipo:
            enemy.GetComponentInParent<JoaoCarlos>()?.TakeDamage(attackDamage);


        }

        // Aqui você pode adicionar animação, som, efeitos visuais etc.
    }

    // Gizmo pra visualizar o alcance do ataque no editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
