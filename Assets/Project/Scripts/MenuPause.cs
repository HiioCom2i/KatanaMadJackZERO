using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class MenuPause : MonoBehaviour
{


    public PlayerMovement player;
    public Slider volumeSFXSlider;
    public Slider volumeMusicaSlider;
    private string parametroVolumeSFX = "Volume SFX";
    private string parametroVolumeMusica = "Volume Música";

    private bool fmodPronto = false;

    IEnumerator Start()
    {
        // Aguarda o sistema da FMOD estar pronto (espera um frame)
        yield return null;
        fmodPronto = true;

        /*if (volumeSFXSlider != null)
        {
            volumeSFXSlider.value = 70f;
            RuntimeManager.StudioSystem.setParameterByName(parametroVolumeSFX, volumeSFXSlider.value);
        }
        if (volumeMusicaSlider != null)
        {
            volumeMusicaSlider.value = 70f;
            RuntimeManager.StudioSystem.setParameterByName(parametroVolumeMusica, volumeMusicaSlider.value);
        }*/
    }

    void Update() { }

    // Botões menu principal
    /*
    public void Opcoes()
    {
        menuOpcoes.SetActive(true);
        menuPrincipal.SetActive(false);
    }
    */
    public void VoltarAoJogo()
    {
        player.pausado = !player.pausado;
        player.TogglePause();
    }

    public void VoltarParaMenu()
    {
        SceneManager.LoadScene("Menu");
    }


    public void AtualizarVolumeSFX()
    {
        if (!fmodPronto) return;

        RuntimeManager.StudioSystem.setParameterByName(parametroVolumeSFX, volumeSFXSlider.value / 100);
        Debug.Log($"Parâmetro global '{parametroVolumeSFX}' ajustado para {volumeSFXSlider.value / 100}");
    }
    
    public void AtualizarVolumeMusica()
    {
        if (!fmodPronto) return;

        RuntimeManager.StudioSystem.setParameterByName(parametroVolumeMusica, volumeMusicaSlider.value/100);
        Debug.Log($"Parâmetro global '{parametroVolumeMusica}' ajustado para {volumeMusicaSlider.value/100}");
    }

}
