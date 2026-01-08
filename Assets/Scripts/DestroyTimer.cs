using UnityEngine;

public class DestroyTimer : MonoBehaviour {
    [SerializeField]
    private float timeToDestroy = 1.0f;
    void Start() => Destroy(gameObject, timeToDestroy);
}