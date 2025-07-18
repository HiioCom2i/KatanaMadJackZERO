using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void Start() { }

    void Update() { }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement_Davi player = other.GetComponent<PlayerMovement_Davi>();

        if (player != null)
        {
            player.health_points = 0;
        }
    }
}
