using UnityEngine;
using UnityEngine.UI;

public class PlayerPauseController : MonoBehaviour
{
    public GameObject menuPauseUI;
    public GameObject hud;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            TogglePause(isPaused);
        }
    }

    void TogglePause(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;

        if (menuPauseUI != null)
            menuPauseUI.SetActive(paused);

        if (hud != null)
            hud.SetActive(!paused);

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void TogglePauseExternamente()
{
    isPaused = !isPaused;
    TogglePause(isPaused);
}
}