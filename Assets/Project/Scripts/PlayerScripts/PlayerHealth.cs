using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public double maxHealth = 200;
    public double currentHealth;
    public float regenInterval = 2f;
    public double regenAmount = 15;
    private bool regenerating = false;

    private bool inCombat = false;

    public GameController gameController;
    public PlayerUIController uiController;  // referência pra atualizar UI

    void Start()
    {
        currentHealth = maxHealth;
        //InvokeRepeating(nameof(RegenerateHealth), regenInterval, regenInterval);
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

        if (currentHealth >= maxHealth)
        {
            CancelInvoke(nameof(RegenerateHealth));
            regenerating = false;
        }
    }

    public void StartRegeneration()
    {
        if (!regenerating && currentHealth < maxHealth)
        {
            regenerating = true;
            InvokeRepeating(nameof(RegenerateHealth), regenInterval, regenInterval);
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

    public void setInCombat(bool valor)
    {
        inCombat = valor;

        if (valor) // Entrou em combate
        {
            CancelInvoke(nameof(RegenerateHealth));
            regenerating = false;
        }
        // Fora de combate: regeneração começa externamente com StartRegeneration()
    }

    void Die()
    {
        SceneManager.LoadScene("GameOver");
    }
}