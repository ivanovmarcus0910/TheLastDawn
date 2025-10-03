using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    [Tooltip("Layer mà enemy sẽ phát hiện (ví dụ: Player)")]
    public LayerMask detectionLayer;

    // Danh sách các collider đang ở trong vùng detection
    public List<Collider2D> detectedColliders = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem collider có nằm trong layer cần detect không
        if (((1 << other.gameObject.layer) & detectionLayer) != 0)
        {
            if (!detectedColliders.Contains(other))
            {
                detectedColliders.Add(other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectedColliders.Contains(other))
        {
            detectedColliders.Remove(other);
        }
    }
}
