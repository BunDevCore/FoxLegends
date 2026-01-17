using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathEndController : MonoBehaviour
{
    public static DeathEndController instance;

    [Header("UI Components")] [SerializeField]
    private TMP_Text timeText;

    [SerializeField] private TMP_Text pointsText;
    
    [Header("UI Images")]
    [SerializeField] private Image normalKeyUI;
    [SerializeField] private Image goldenKeyUI;
    [SerializeField] private Image diamondKeyUI;

    [Header("Sprites")]
    [SerializeField] private Sprite normalKeySprite;
    [SerializeField] private Sprite goldenKeySprite;
    [SerializeField] private Sprite diamondKeySprite;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Duplicated Death End Manager", gameObject);
    }

    public void LevelDeath()
    {
        timeText.text = GameManager.instance.timerText.text;
        pointsText.text = GameManager.instance.pointsText.text;
        if (KeyManager.instance.hasNormalKey)
            normalKeyUI.sprite = normalKeySprite;
        if (KeyManager.instance.hasGoldenKey)
            goldenKeyUI.sprite = goldenKeySprite;
        if (KeyManager.instance.hasDiamondKey)
            diamondKeyUI.sprite = diamondKeySprite;
    }
}