using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class RankMaskUI : MonoBehaviour
{
    public RectMask2D rectMask; // Referência ao componente "S mask"

    [Range(0f, 1f)]
    public float fillSlider = 0.5f;

    public float maxPaddingTop = 270f; // Altura total da imagem/máscara

    void Update()
    {

        // Calcula o valor proporcional de preenchimento
        float paddingTop = Mathf.Lerp(maxPaddingTop, 0f, fillSlider);

        // Usa reflection para acessar e alterar o padding do RectMask2D
        var paddingField = typeof(RectMask2D).GetField("m_Padding", BindingFlags.NonPublic | BindingFlags.Instance);
        if (paddingField != null)
        {
            RectOffset padding = paddingField.GetValue(rectMask) as RectOffset;
            if (padding != null)
            {
                padding.top = Mathf.RoundToInt(paddingTop);
                rectMask.enabled = false;  // Força atualizar o mask
                rectMask.enabled = true;
            }
        }
    }
}
