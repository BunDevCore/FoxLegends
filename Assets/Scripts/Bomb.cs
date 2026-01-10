using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject bombExplosionPrefab;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            Instantiate(bombExplosionPrefab, transform.position + Vector3.up*0.2f, transform.rotation);
            Destroy(gameObject);
            if (other.CompareTag("Player"))
            {
                other.gameObject.SendMessage("PlayerDeath");
            }
        }
    }
}