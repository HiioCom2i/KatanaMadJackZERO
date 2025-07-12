using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController_Davi : MonoBehaviour
{

    public GameObject menuPrincipal;
    public GameObject menuOpcoes;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    //Botões menu principal
    public void ComecarJogo()
    {
        SceneManager.LoadScene("LevelZERO");
    }

    public void Opcoes()
    {
        menuOpcoes.SetActive(true); // Mostra o canvas de opções
        menuPrincipal.SetActive(false);   // Esconde o menu principal (opcional)
    }

    public void VoltarParaMenu()
    {
        menuOpcoes.SetActive(false); // Esconde o canvas de opções
        menuPrincipal.SetActive(true);   // Mostra o menu principal (opcional)
    }

    public void Sair()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}
