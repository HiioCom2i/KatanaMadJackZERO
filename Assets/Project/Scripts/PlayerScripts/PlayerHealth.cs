using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public double maxHealth = 200;
    public double currentHealth;
    public float regenInterval = 5f;
    public double regenAmount = 5;

    public GameController gameController;
    public PlayerUIController uiController;  // referência pra atualizar UI

    void Start()
    {
        currentHealth = maxHealth;
        InvokeRepeating(nameof(RegenerateHealth), regenInterval, regenInterval);
        UpdateUI();
    }

    public void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += regenAmount;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            UpdateUI();
        }
    }

    public void TakeDamage(double damage)
    {
        currentHealth -= damage;
        gameController.addPlayerPoints(-60);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (uiController != null)
            uiController.UpdateHealth(currentHealth);
    }

    void Die()
    {
        SceneManager.LoadScene("GameOver");
    }
}