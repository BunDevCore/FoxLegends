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
        }
    }
    
    public void OnResumeClicked() => InGame();
    public void OnRestartClicked() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void OnReturnToMainClicked() => SceneManager.LoadScene("MainMenu");
    public void OnSettingsClicked()
    {
        settingsCanvas.enabled = true;
        Time.timeScale = 0;
        SetGameState(GameState.OPTIONS);
    }
    
    public void OnSettingsBackClicked()
    {
        settingsCanvas.enabled = false;
        Time.timeScale = 1;
        SetGameState(GameState.PAUSE_MENU);
    }
    
    public void OnQualityUpClicked()
    {
        QualitySettings.IncreaseLevel();
        // qualityText.SetText("Quality: "+QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

    public void OnQualityDownClicked()
    {
        QualitySettings.DecreaseLevel();
        // qualityText.SetText("Quality: "+QualitySettings.names[QualitySettings.GetQualityLevel()]);
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
    }

    public void InGame()
    {
        SetGameState(GameState.GAME);
    }
}