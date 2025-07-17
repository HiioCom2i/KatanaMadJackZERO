using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{

    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Destrava o cursor
        Cursor.visible = true;                  // Torna o cursor visível
    }
    void Update(){}

    public void TentarNovamente()
    {
        SceneManager.LoadScene("LevelZERO");
    }

    public void VoltarAoMenuPrincipal()
    {
        SceneManager.LoadScene("Menu");
    }
}
