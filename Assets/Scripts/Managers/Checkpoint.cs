using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private SpriteRenderer spriteRenderer;
    private bool isActivated = false;

    [Header("Camera limits")]
    public bool useCameraLimits = true;
    public Vector2 minXandY;
    public Vector2 maxXandY;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {   
            ResetAllCheckpoints();
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        isActivated = true;
        
        if (activeSprite != null)
        {
            spriteRenderer.sprite = activeSprite;
        }
        if (useCameraLimits) 
            GameManager.instance.UpdateSpawnPoint(transform.position + new Vector3(0, 0.1f, 0));
        else 
            GameManager.instance.UpdateSpawnPoint(transform.position + new Vector3(0, 0.1f, 0), minXandY, maxXandY);
    }
    
    public void ResetCheckpoint()
    {
        isActivated = false;
        if (inactiveSprite != null) spriteRenderer.sprite = inactiveSprite;
    }

    private void ResetAllCheckpoints()
    {
        Checkpoint[] allCheckpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        
        foreach (Checkpoint cp in allCheckpoints)
        {
            cp.ResetCheckpoint();
        }
    }
}