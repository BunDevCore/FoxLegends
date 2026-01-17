using UnityEngine;
using UnityStandardAssets._2D;

public class EditCameraTrigger : MonoBehaviour
{
    public Vector2 maxXAndY;
    public Vector2 minXAndY;

    private CameraFollow mCameraFollow;

    private void Awake()
    {
        mCameraFollow = Camera.main.GetComponent<CameraFollow>();

        if (mCameraFollow == null)
        {
            Debug.LogError("Nie znaleziono skryptu CameraFollow na Main Camera!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mCameraFollow.maxXAndY = maxXAndY;
            mCameraFollow.minXAndY = minXAndY;
        }
    }
}