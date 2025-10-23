using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public SpriteRenderer currentBackground;
    public float smooth = 0.15f;

    private float minX, maxX, minY, maxY;
    private float camHalfW, camHalfH;

    void Start()
    {
        var cam = Camera.main;
        camHalfH = cam.orthographicSize;
        camHalfW = camHalfH * cam.aspect;
        SetBounds(currentBackground);
    }

    void LateUpdate()
    {
        if (!target) return;

        float tx = Mathf.Clamp(target.position.x, minX, maxX);
        float ty = Mathf.Clamp(target.position.y, minY, maxY);
        Vector3 targetPos = new Vector3(tx, ty, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, smooth);
    }

    public void SetBounds(SpriteRenderer bg)
    {
        currentBackground = bg;
        Bounds b = bg.bounds;
        minX = b.min.x + camHalfW;
        maxX = b.max.x - camHalfW;
        minY = b.min.y + camHalfH;
        maxY = b.max.y - camHalfH;
        print("Đã setBound trong Camera" + bg.name);

    }
}
