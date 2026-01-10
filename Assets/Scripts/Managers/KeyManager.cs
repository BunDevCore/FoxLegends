using UnityEngine;
using UnityEngine.UI;

public class KeyManager : MonoBehaviour
{
    public static KeyManager instance;

    [Header("UI Images")]
    [SerializeField] private Image normalKeyUI;
    [SerializeField] private Image goldenKeyUI;
    [SerializeField] private Image diamondKeyUI;

    [Header("Sprites")]
    [SerializeField] private Sprite normalKeySprite;
    [SerializeField] private Sprite goldenKeySprite;
    [SerializeField] private Sprite diamondKeySprite;

    public bool hasNormalKey = false;
    public bool hasGoldenKey = false;
    public bool hasDiamondKey = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Duplicated Key Manager", gameObject);
    }

    public void CollectKey(string color)
    {
        switch (color)
        {
            case "Normal":
                hasNormalKey = true;
                normalKeyUI.sprite = normalKeySprite;
                break;
            case "Golden":
                hasGoldenKey = true;
                goldenKeyUI.sprite = goldenKeySprite;
                break;
            case "Diamond":
                hasDiamondKey = true;
                diamondKeyUI.sprite = diamondKeySprite;
                break;
        }
    }
}