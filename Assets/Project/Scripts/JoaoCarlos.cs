using UnityEngine;

public class JoaoCarlos : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} tomou {damage} de dano. Vida restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} morreu.");
        Destroy(gameObject); // ou uma animação de morte, som etc
    }
}