using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("ATACOU");
        // Aqui você coloca a lógica do ataque (hitbox, dano, animação, som...)
    }
}