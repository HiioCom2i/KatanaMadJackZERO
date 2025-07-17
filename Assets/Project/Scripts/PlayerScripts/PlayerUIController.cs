using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public Text healthPointsText;

    public void UpdateHealth(double currentHealth)
    {
        if (healthPointsText != null)
            healthPointsText.text = currentHealth.ToString("F0");  // sem casas decimais
    }
}