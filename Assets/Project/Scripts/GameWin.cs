using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou na trigger é o player
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("GameWin");
        }
    }
}
