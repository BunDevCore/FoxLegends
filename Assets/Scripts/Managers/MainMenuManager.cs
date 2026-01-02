using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuManager : MonoBehaviour
{
    public Canvas mainMenuCanvas;
    public Canvas settingsCanvas;
    public TMP_Text qualityText;

    private void Start()
    {
        mainMenuCanvas.enabled = true;
        settingsCanvas.enabled = false;
        qualityText.SetText("Quality:\n"+QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    public void OnStartButtonPressed() => SceneManager.LoadScene("Level1");

    public void OnSettingsClicked()
    {
        mainMenuCanvas.enabled = false;
        settingsCanvas.enabled = true;
    }
    
    public void OnSettingsBackClicked()
    {
        mainMenuCanvas.enabled = true;
        settingsCanvas.enabled = false;
    }

    public void OnExitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    
    public void OnQualityUpClicked()
    {
        QualitySettings.IncreaseLevel();
        qualityText.SetText("Quality:\n"+QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    public void OnQualityDownClicked()
    {
        QualitySettings.DecreaseLevel();
        qualityText.SetText("Quality:\n"+QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
    }
}