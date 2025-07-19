using FMOD.Studio;
using FMODUnity;
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

    // VARIÁVEIS FMOD
    private EventInstance passos;

    void Start()
    {
        speed = baseSpeed * speedMultiplier;
        controller = GetComponent<CharacterController>();

        passos = RuntimeManager.CreateInstance("event:/Player_Passos");  // Inicia evento dos passos
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        move = transform.right * moveX + transform.forward * moveZ;

        if (move.magnitude > 0.1f && controller.isGrounded)
        {
            // Enquanto o jogador está se movendo toca som de passos
            FMOD.Studio.PLAYBACK_STATE playbackState;
            passos.getPlaybackState(out playbackState);

            if (playbackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                passos.start();
            }
        }
        else
        {
            passos.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

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

    void OnDestroy()
    {
        passos.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        passos.release();
    }
}