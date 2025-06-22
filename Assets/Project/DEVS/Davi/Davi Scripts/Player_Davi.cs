using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Davi : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private double health_points = 200;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 move;
    public GameController gameController;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        InvokeRepeating("playerHPRegen", 5f, 5f); // Player ganha vida a cada 5 segundos
    }

    public void playerHPRegen()
    {
        if (health_points < 200)
        {
            health_points += 5;
        }
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        move = transform.right * moveX + transform.forward * moveZ;

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // ataque
        if (Input.GetButtonDown("Fire1"))
        {
            PlayerAttack();
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        Vector3 finalMove = move * speed + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void playerTakesDamage(double damage)
    {
        health_points -= damage;
        gameController.addPlayerPoints(-60); // Player perde 60 pontos ao tomar dano
    }

    private void PlayerAttack()
    {
        Debug.Log("ATACOU");
    }
}
