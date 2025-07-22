using UnityEngine;

public class EnemySpriteSwitcher : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public GameObject frontSprite;
    public GameObject backSprite;

    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = (player.position - transform.position).normalized;
        Vector3 forward = transform.forward;

        float angle = Vector3.Angle(forward, toPlayer);

        // Mostrar frente se o ângulo for de 90° ou menos (na frente)
        bool isFacingPlayer = angle <= 90f;

        frontSprite.SetActive(isFacingPlayer);
        backSprite.SetActive(!isFacingPlayer);

        // Opcional: girar sprite para sempre olhar para a câmera (como em Doom)
        // Isso faz o sprite "encarar" o jogador sempre
        
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y; // mantém o sprite nivelado no Y
        transform.LookAt(lookPos);
        
    }
}
