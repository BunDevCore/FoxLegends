using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using JetBrains.Annotations;
using UnityStandardAssets._2D;

public enum GameState
{
    [InspectorName("Gameplay")] GAME,
    [InspectorName("Pause")] PAUSE_MENU,
    [InspectorName("Options")] SETTINGS,
    [InspectorName("Level death")]
    LEVEL_DEATH,
    [InspectorName("Level completed successfully")]
    LEVEL_COMPLETED
}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.PAUSE_MENU;
    public static GameManager instance;

    [Header("UI References")] public Canvas inGameCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas settingsCanvas;
    public Canvas endingCanvas;
    public Canvas deathEndingCanvas;
    public TMP_Text qualityText;
    public TMP_Text timerText;
    public TMP_Text pointsText;
    public Slider soundSlider;
    public Slider shakeSlider;

    [Header("Checkpoint System")] public Vector3 currentSpawnPoint;

    public GameObject[] hearts;
    private int lives;

    [Header("Score System")] public float timeToComplete = 100;
    public float currentTime = 0;
    public int points;

    [Header("Cursor Manager")] public CursorManager cursorManager;


    [Header("Fading")] public Image blackoutImage;
    public float fadeSpeed = 2f;
    private CameraFollow mCameraFollow;

    private Vector2 lastMinXandY;
    private Vector2 lastMaxXandY;
    private bool runTime = false;

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
        mCameraFollow = Camera.main?.GetComponent<CameraFollow>();
        lastMinXandY = mCameraFollow?.minXAndY ?? Vector2.zero;
        lastMaxXandY = mCameraFollow?.maxXAndY ?? Vector2.zero;
        settingsCanvas.enabled = false;
        qualityText.SetText("Quality:\n" + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        lives = 5;
        UpdateHeartsUI();
        points = 0;
        UpdatePointsUI();
        if (soundSlider)
        {
            soundSlider.value = AudioListener.volume;
            soundSlider.onValueChanged.AddListener(v => AudioListener.volume = v);
        }
        if (shakeSlider)
        {
            shakeSlider.value = PlayerPrefs.GetFloat("ShakeIntensity", 0.5f);
            shakeSlider.onValueChanged.AddListener(v => ShakeIntensity = v);
        }

        runTime = true;
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

        if (runTime)
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
        Time.timeScale = 1;
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

    public static float ShakeIntensity
    {
        get => PlayerPrefs.GetFloat("ShakeIntensity", 0.5f);
        set
        {
            PlayerPrefs.SetFloat("ShakeIntensity", value);
            PlayerPrefs.Save();
        }
    }

    void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        inGameCanvas.enabled = currentGameState == GameState.GAME;
        pauseMenuCanvas.enabled = currentGameState == GameState.PAUSE_MENU;
        settingsCanvas.enabled = currentGameState == GameState.SETTINGS;
        deathEndingCanvas.enabled = currentGameState == GameState.LEVEL_DEATH;
        endingCanvas.enabled = currentGameState == GameState.LEVEL_COMPLETED;
    }

    public void PauseMenu()
    {
        // cursorManager.ShowCursor();
        cursorManager.ResetCursor();
        SetGameState(GameState.PAUSE_MENU);
        Time.timeScale = 0;
    }

    public void InGame()
    {
        // cursorManager.HideAndResetCursor();
        SetGameState(GameState.GAME);
        if (!DialogueManager.instance || !DialogueManager.instance.IsActive)
            Time.timeScale = 1;
    }

    public void LevelDeath()
    {
        DeathEndController.instance.LevelDeath();
        SetGameState(GameState.LEVEL_DEATH);
        Time.timeScale = 0;
        runTime = false;
    }
    
    public void LevelComplete()
    {
        EndController.instance.LevelComplete();
        SetGameState(GameState.LEVEL_COMPLETED);
        Time.timeScale = 0;
        runTime = false;
    }

    public void UpdateSpawnPoint(Vector3 newPosition)
    {
        lastMinXandY = mCameraFollow.minXAndY;
        lastMaxXandY = mCameraFollow.maxXAndY;
        currentSpawnPoint = newPosition;
    }

    public void UpdateSpawnPoint(Vector3 newPosition, Vector2 min, Vector2 max)
    {
        lastMinXandY = min;
        lastMaxXandY = max;
        currentSpawnPoint = newPosition;
    }

    public void AddLives(int liveChange)
    {
        lives += liveChange;
        lives = Mathf.Clamp(lives, 0, hearts.Length);

        if (lives <= 0)
            LevelDeath();

        UpdateHeartsUI();
    }

    private void UpdateHeartsUI()
    {
        if (hearts == null || hearts.Length == 0) return;
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < lives);
        }
    }

    public void AddPoints(int newPoints)
    {
        points += newPoints;
        UpdatePointsUI();
    }

    private void UpdatePointsUI()
    {
        pointsText.SetText($"{points:000}");
    }

    public IEnumerator RespawnSequence(PlayerController player)
    {
        player.isDead = true;
        player.enableMovement = false;
        player.rigidBody.linearVelocity = Vector2.zero;
        player.rigidBody.simulated = false;
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        mCameraFollow.enableSmoothing = false;
        player.transform.position = currentSpawnPoint;
        player.rigidBody.linearVelocity = Vector2.zero;
        mCameraFollow.minXAndY = lastMinXandY;
        mCameraFollow.maxXAndY = lastMaxXandY;
        player.animator.SetBool("isDead", false);
        yield return new WaitForSeconds(0.3f);
        mCameraFollow.enableSmoothing = true;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            blackoutImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        player.isDead = false;
        player.enableMovement = true;
        player.rigidBody.simulated = true;
    }
}