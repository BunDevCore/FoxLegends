using System;
using TMPro;
using UnityEngine;

public class EndController : MonoBehaviour
{
    public static EndController instance;
    [Header("Settings")] [SerializeField] private float scoreTimeDivider;

    [Header("UI Components")] [SerializeField]
    private TMP_Text timeText;

    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private const String keyHighScore = "HighScoreLevel1";

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Duplicated End Manager", gameObject);
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(keyHighScore))
            PlayerPrefs.SetInt(keyHighScore, 0);
    }

    public void LevelComplete()
    {
        timeText.text = GameManager.instance.timerText.text;
        pointsText.text = GameManager.instance.pointsText.text;
        int score = Math.Max(0,
            (int)(GameManager.instance.points - GameManager.instance.currentTime / scoreTimeDivider));
        scoreText.text = $"{score:000}";
        int highScore = PlayerPrefs.GetInt(keyHighScore);
        if (highScore < score)
        {
            highScore = score;
            PlayerPrefs.SetInt(keyHighScore, highScore);
        }

        highscoreText.text = $"{highScore:000}";
    }
}