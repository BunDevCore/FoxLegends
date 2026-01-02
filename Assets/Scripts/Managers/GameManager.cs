using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameState
{
    [InspectorName("Gameplay")] GAME,
    [InspectorName("Pause")] PAUSE_MENU,
    [InspectorName("Options")] OPTIONS,

    [InspectorName("Level completed (either successfully or failed)")]
    LEVEL_COMPLETED
}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.PAUSE_MENU;
    public static GameManager instance;
    
    public Canvas inGameCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas settingsCanvas;
    public TMP_Text qualityText;
    public TMP_Text timerText;
    private float currentTime = 0;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Duplicated Game Manager", gameObject);

        InGame();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settingsCanvas.enabled = false;
        qualityText.SetText("Quality:\n"+QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.PAUSE_MENU)
                InGame();
            else if (currentGameState == GameState.GAME)
                PauseMenu();
            else if (currentGameState == GameState.OPTIONS) 
                SetGameState(GameState.PAUSE_MENU);
        }

        tickTime();
    }

    void tickTime()
    {
        currentTime += Time.deltaTime;
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        float milliseconds = (currentTime * 1000) % 1000;
        timerText.text = string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
    
    public void OnResumeClicked() => InGame();
    public void OnRestartClicked() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void OnReturnToMainClicked() => SceneManager.LoadScene("MainMenu");
    public void OnSettingsClicked()
    {
        settingsCanvas.enabled = true;
        SetGameState(GameState.OPTIONS);
    }
    
    public void OnSettingsBackClicked()
    {
        settingsCanvas.enabled = false;
        SetGameState(GameState.PAUSE_MENU);
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
    
    void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        inGameCanvas.enabled = currentGameState == GameState.GAME;
        pauseMenuCanvas.enabled = currentGameState == GameState.PAUSE_MENU;
    }

    public void PauseMenu()
    {
        SetGameState(GameState.PAUSE_MENU);
        Time.timeScale = 0;
    }

    public void InGame()
    {
        SetGameState(GameState.GAME);
        Time.timeScale = 1;
    }
}