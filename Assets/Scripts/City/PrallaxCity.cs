using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float startPosX;
    private Transform cam;

    [Range(0f, 1f)]
    public float parallaxEffect; // 0 = không di chuyển, 1 = di chuyển như foreground

    void Start()
    {
        cam = Camera.main.transform;
        startPosX = transform.position.x;
    }

    void LateUpdate()
    {
        float distX = (cam.position.x - startPosX) * parallaxEffect;
        transform.position = new Vector3(startPosX + distX, transform.position.y, transform.position.z);
    }
}
