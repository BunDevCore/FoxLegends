using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    private Camera camCamera;
    public float parallaxEffect;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        startpos = transform.position.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
        length = spriteRenderer.bounds.size.x;
        camCamera = cam.GetComponent<Camera>();
    }

    void Update()
    {
        float cameraHeight = camCamera.orthographicSize * 2f;
        float baseSpriteHeight = spriteRenderer.sprite.bounds.size.y;
        float targetScaleY = cameraHeight / baseSpriteHeight;
        transform.localScale = new Vector3(targetScaleY, targetScaleY, 1f);
        
        float temp = cam.transform.position.x * (1 - parallaxEffect);
        float dist = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startpos + dist + targetScaleY, cam.transform.position.y, transform.position.z);

        float currentWidth = length * transform.localScale.x;
        if (temp > startpos + currentWidth) startpos += currentWidth;
        else if (temp < startpos - currentWidth) startpos -= currentWidth;
    }
}