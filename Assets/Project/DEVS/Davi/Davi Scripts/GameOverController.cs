using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
   
    void Start(){}
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
