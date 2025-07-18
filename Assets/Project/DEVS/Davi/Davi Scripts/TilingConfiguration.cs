using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TilingConfiguration : MonoBehaviour
{
    [Header("Tiling personalizado para URP")]
    public float tilingX = 1f;
    public float tilingY = 1f;

    private Renderer rend;
    private MaterialPropertyBlock block;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    void Start()
    {
        ApplyTiling();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;
        ApplyTiling();
    }

    void ApplyTiling()
    {
        rend.GetPropertyBlock(block);

        // "_BaseMap_ST" é usado pelo URP para controlar Tiling e Offset da textura principal
        block.SetVector("_BaseMap_ST", new Vector4(tilingX, tilingY, 0f, 0f));

        rend.SetPropertyBlock(block);
    }
}
