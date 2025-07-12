using UnityEngine;
using UnityEngine.UI;

public class RankLetterFill : MonoBehaviour
{
    public Image brightLetterImage; // A imagem clara da letra com Image.Type = Filled
    [Range(0f, 1f)]
    public float fillAmount = 0f;

    void Update()
    {
        brightLetterImage.fillAmount = fillAmount;
    }

    public void setFillAmount(float max, float min, float player_points)
    {

        float f = (player_points - min) / (max - min);
        fillAmount = f;
    }
}
