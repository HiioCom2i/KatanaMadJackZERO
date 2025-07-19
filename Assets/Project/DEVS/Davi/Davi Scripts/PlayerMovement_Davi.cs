using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement_Davi : MonoBehaviour
{

    // VARIÁVEIS MOVIMENTO 
    public float base_speed = 5f;
    private float player_speed_multiplier = 1;
    private float player_speed;
    public float gravity = -9.81f;
    public float jumpHeight = 0.6f;
    public double health_points = 200;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 move;
    public GameController_Davi gameController;

    // VARIÁVEIS FMOD
    private EventInstance passos;


    // VARIÁVEIS DASH 
    public float dashDistance = 6f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.8f;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 dashDirection;
    private float dashTimer = 0f;

    // PAUSE
    public bool pausado = false;
    public GameObject menuPauseUI;
    public GameObject hud;
    public Text health_points_UI;

    void Start()
    {

        passos = RuntimeManager.CreateInstance("event:/Player_Passos");

        player_speed = player_speed_multiplier * base_speed;
        controller = GetComponent<CharacterController>();
        InvokeRepeating("playerHPRegen", 5f, 5f);
        if (menuPauseUI != null)
            menuPauseUI.SetActive(false);
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

        if (health_points <= 0)
        {
            PlayerDies();
        }

        // Gerencia o pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausado = !pausado;
            TogglePause();
        }


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

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            PlayerAttack();
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        Vector3 finalMove = move * player_speed + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);

        health_points_UI.text = health_points.ToString();
    }

    private void StartDash()
    {
        canDash = false;
        isDashing = true;
        velocity.y = 0f;

        if (move.magnitude > 0.1f)
        {
            dashDirection = move.normalized;
        }
        else
        {
            dashDirection = transform.forward;
        }

        dashDirection.y = 0f;
    }

    private void ResetDash()
    {
        canDash = true;
    }

    public void playerTakesDamage(double damage)
    {
        health_points -= damage;
        gameController.addPlayerPoints(-60);
    }

    private void PlayerAttack()
    {
        Debug.Log("ATACOU");
    }

    public void setPlayerSpeedMultiplier(float s)
    {
        player_speed_multiplier = s;
        player_speed = player_speed_multiplier * base_speed;
    }

    public void TogglePause()
    {
        Time.timeScale = pausado ? 0f : 1f;
        if (menuPauseUI != null)
        {
            menuPauseUI.SetActive(pausado);
        }
        if (hud != null)
        {
            hud.SetActive(!pausado);
        }
        Cursor.lockState = pausado ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pausado;
    }

    public void PlayerDies()
    {
        SceneManager.LoadScene("GameOver");
    }

    void OnDestroy()
    {
        passos.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        passos.release();
    }

}
