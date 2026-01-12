using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")] 
    public Canvas mainMenuCanvas;
    public Canvas settingsCanvas;
    public TMP_Text qualityText;
    public Slider soundSlider;
    public Slider shakeSlider;

    [Header("Cursor Manager")]
    public CursorManager cursorManager;

    private void Start()
    {
        mainMenuCanvas.enabled = true;
        settingsCanvas.enabled = false;
        qualityText.SetText("Quality:\n" + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        soundSlider.value = AudioListener.volume;
        soundSlider.onValueChanged.AddListener(v => AudioListener.volume = v);
        shakeSlider.value = PlayerPrefs.GetFloat("ShakeIntensity", 0.5f);
        shakeSlider.onValueChanged.AddListener(v => ShakeIntensity = v);
    }

    public void OnStartButtonPressed() => SceneManager.LoadScene("Level1");

    public void OnSettingsClicked()
    {
        cursorManager.ResetCursor();
        mainMenuCanvas.enabled = false;
        settingsCanvas.enabled = true;
    }

    public void OnSettingsBackClicked()
    {
        cursorManager.ResetCursor();
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
        qualityText.SetText("Quality:\n" + QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    public void OnQualityDownClicked()
    {
        QualitySettings.DecreaseLevel();
        qualityText.SetText("Quality:\n" + QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    public static float ShakeIntensity
    {
        get => PlayerPrefs.GetFloat("ShakeIntensity", 0.5f); 
        set 
        {
            PlayerPrefs.SetFloat("ShakeIntensity", value);
            PlayerPrefs.Save();
        }
    }
}