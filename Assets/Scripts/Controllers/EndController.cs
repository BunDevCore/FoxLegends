using TMPro;
using UnityEngine;

public class EndController : MonoBehaviour
{
    public static EndController instance;
    
    [Header("UI Components")]
    [SerializeField] private TMP_Text time;
    [SerializeField] private TMP_Text points;
    [SerializeField] private TMP_Text highscore;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Duplicated End Manager", gameObject);
    }

    public void LevelComplete()
    {
        GameManager.instance.LevelComplete();
        time.text = GameManager.instance.timerText.text;
        points.text = GameManager.instance.pointsText.text;
        highscore.text = GameManager.instance.pointsText.text;
    }
    
}