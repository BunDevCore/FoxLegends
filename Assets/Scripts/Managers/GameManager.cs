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
    [InspectorName("Options")] SETTINGS,

    [InspectorName("Level completed (either successfully or failed)")]
    LEVEL_COMPLETED
}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.PAUSE_MENU;
    public static GameManager instance;

    [Header("UI References")]
    public Canvas inGameCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas settingsCanvas;
    public TMP_Text qualityText;
    public TMP_Text timerText;

    [Header("Checkpoint System")]
    public Vector3 currentSpawnPoint;

    public GameObject[] hearts;
    private float currentTime = 0;
    private int lives;

    [Header("Cursor Manager")]
    public CursorManager cursorManager;

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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            currentSpawnPoint = player.transform.position;
        settingsCanvas.enabled = false;
        qualityText.SetText("Quality:\n" + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        lives = hearts.Length;
        UpdateHeartsUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.PAUSE_MENU)
                InGame();
            else if (currentGameState == GameState.GAME || currentGameState == GameState.SETTINGS)
                PauseMenu();
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

    public void OnReturnToMainClicked()
    {
        cursorManager.ResetCursor();
        SceneManager.LoadScene("MainMenu");
    }

    public void OnSettingsClicked()
    {
        cursorManager.ResetCursor();
        settingsCanvas.enabled = true;
        SetGameState(GameState.SETTINGS);
    }

    public void OnSettingsBackClicked()
    {
        cursorManager.ResetCursor();
        settingsCanvas.enabled = false;
        SetGameState(GameState.PAUSE_MENU);
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

    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
    }

    void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        inGameCanvas.enabled = currentGameState == GameState.GAME;
        pauseMenuCanvas.enabled = currentGameState == GameState.PAUSE_MENU;
        settingsCanvas.enabled = currentGameState == GameState.SETTINGS;
    }

    public void PauseMenu()
    {
        cursorManager.ShowCursor();
        SetGameState(GameState.PAUSE_MENU);
        Time.timeScale = 0;
    }

    public void InGame()
    {
        cursorManager.HideAndResetCursor();
        SetGameState(GameState.GAME);
        Time.timeScale = 1;
    }

    public void UpdateSpawnPoint(Vector3 newPosition)
    {
        currentSpawnPoint = newPosition;
    }

    public void AddLives(int liveChange)
    {
        lives += liveChange;
        lives = Mathf.Clamp(lives, 0, hearts.Length);

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
            OnRestartClicked();
        }

        UpdateHeartsUI();
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < lives)
                hearts[i].SetActive(true);
            else
                hearts[i].SetActive(false);
        }
    }
}