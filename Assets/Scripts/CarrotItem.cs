using UnityEngine;

public class CarrotItem : MonoBehaviour
{
    [SerializeField] private string keyColor;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            KeyManager.instance.CollectKey(keyColor);
            Destroy(gameObject);
        }
    }
}