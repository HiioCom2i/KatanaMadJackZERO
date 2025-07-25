using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerMovement))]
public class PlayerDash : MonoBehaviour
{
    public float dashDistance = 6f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.8f;

    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 dashDirection;
    private float dashTimer = 0f;

    private CharacterController controller;
    private PlayerMovement playerMovement;

    // VARIÁVEIS FMOD
    private EventInstance dash;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        dash = RuntimeManager.CreateInstance("event:/Player_Dash");
    }

    void Update()
    {
        if (isDashing)
        {
            controller.Move(dashDirection * (dashDistance / dashDuration) * Time.deltaTime);
            dashTimer += Time.deltaTime;

            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                dashTimer = 0f;
                Invoke(nameof(ResetDash), dashCooldown);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        dash.start(); // Toca som de dash
        canDash = false;
        isDashing = true;

        Vector3 moveDir = playerMovement.GetMoveDirection();
        dashDirection = moveDir.magnitude > 0.1f ? moveDir : transform.forward;
        dashDirection.y = 0f;
    }

    private void ResetDash()
    {
        canDash = true;
    }

    void OnDestroy()
    {
        dash.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        dash.release();
    }
}