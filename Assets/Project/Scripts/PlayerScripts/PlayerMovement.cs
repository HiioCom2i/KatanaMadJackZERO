using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float baseSpeed = 5f;
    private float speedMultiplier = 1f;
    private float speed;
    public float gravity = -9.81f;
    public float jumpHeight = 0.6f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 move;

    void Start()
    {
        speed = baseSpeed * speedMultiplier;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        move = transform.right * moveX + transform.forward * moveZ;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        Vector3 finalMove = move * speed + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
        speed = baseSpeed * speedMultiplier;
    }

    public Vector3 GetMoveDirection()
    {
        return move.normalized;
    }
}