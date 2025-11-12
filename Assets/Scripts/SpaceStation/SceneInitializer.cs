using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    [Header("Cấu hình Scene")]
    public CameraScript cameraScript;       // Kéo Main Camera vào đây
    public SpriteRenderer stationBackground; // Kéo ảnh nền trạm không gian vào đây
    public Transform spawnPoint;            // Điểm spawn (Empty GameObject)

    void Start()
    {
        // 1. Tìm Player Singleton (đã DontDestroyOnLoad)
        PlayerBase player = PlayerBase.Instance;

        if (player != null)
        {
            // 2. Đặt vị trí Player
            // Tạm tắt vật lý để dịch chuyển không bị lỗi va chạm
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb) rb.simulated = false;

            // Nếu có lưu tên điểm spawn trong GameSession thì tìm, không thì dùng mặc định
            if (!string.IsNullOrEmpty(MapManager.nextSpawnPointName))
            {
                var targetPoint = GameObject.Find(MapManager.nextSpawnPointName);
                if (targetPoint) player.transform.position = targetPoint.transform.position;
                else player.transform.position = spawnPoint.position;
            }
            else
            {
                player.transform.position = spawnPoint.position;
            }

            if (rb) rb.simulated = true;

            // 3. CẤU HÌNH CAMERA (Phần quan trọng)
            if (cameraScript != null)
            {
                // Gán target là Player vừa tìm thấy
                cameraScript.target = player.transform;

                // Gán giới hạn background của trạm không gian
                cameraScript.SetBounds(stationBackground);

                // Dịch chuyển Camera ngay lập tức đến Player (tránh hiệu ứng trượt từ xa tới)
                Vector3 startPos = player.transform.position;
                startPos.z = cameraScript.transform.position.z;
                cameraScript.transform.position = startPos;
            }
        }
    }
}