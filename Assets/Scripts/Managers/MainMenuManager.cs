using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")] 
    public Canvas startCanvas;
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
        startCanvas.enabled = false;
        qualityText.SetText("Quality:\n" + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        soundSlider.value = AudioListener.volume;
        soundSlider.onValueChanged.AddListener(v => AudioListener.volume = v);
        shakeSlider.value = PlayerPrefs.GetFloat("ShakeIntensity", 0.5f);
        shakeSlider.onValueChanged.AddListener(v => ShakeIntensity = v);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!mainMenuCanvas.enabled)
                OnBackToMenuClicked();
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            if (mainMenuCanvas.enabled)
                OnStartButtonPressed();
        }
    }

    private void StartGame(DifficultyLevel dl)
    {
        GlobalDifficulty.Difficulty = dl;
        SceneManager.LoadScene("Level1");
    }
    
    public void OnEasyPressed() => StartGame(DifficultyLevel.Easy);
    public void OnNormalPressed() => StartGame(DifficultyLevel.Normal);
    public void OnHardcorePressed() => StartGame(DifficultyLevel.Hardcore);

    public void OnStartButtonPressed()
    {
        mainMenuCanvas.enabled = false;
        settingsCanvas.enabled = false;
        startCanvas.enabled = true;
    }

    public void OnSettingsClicked()
    {
        cursorManager.ResetCursor();
        mainMenuCanvas.enabled = false;
        settingsCanvas.enabled = true;
        startCanvas.enabled = false;
    }

    public void OnBackToMenuClicked()
    {
        cursorManager.ResetCursor();
        mainMenuCanvas.enabled = true;
        settingsCanvas.enabled = false;
        startCanvas.enabled = false;
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