using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PortalGate : MonoBehaviour
{
    public MapManager mapManager;  // kéo MapManager vào
    public GateSide side;          // chọn Left hoặc Right trong Inspector

    void Reset()
    {
        // đảm bảo collider là trigger
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // gọi MapManager để xử lý chuyển map
        mapManager.Travel(side);
    }
}