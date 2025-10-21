using UnityEngine;

public enum GateSide { Left, Right }

[System.Serializable]
public class MapInfo
{
    public string name;
    public GameObject mapRoot;          // GO chứa BG + tile của map
    public SpriteRenderer background;   // BG dùng tính bounds
    public Transform leftSpawn;         // điểm spawn khi từ map bên trái đi sang
    public Transform rightSpawn;        // điểm spawn khi từ map bên phải đi sang
    public int leftNeighbor = -1;       // index map bên trái (không có = -1)
    public int rightNeighbor = -1;      // index map bên phải
}

public class MapManager : MonoBehaviour
{
    public MapInfo[] maps;
    public int currentIndex = 0;

    [Header("Refs")]
    public Transform player;
    public PlayerBase playerScript;         // có SetBounds(SpriteRenderer)
    public CameraScript cameraFollow;   // có SetBounds(SpriteRenderer)

    void Start()
    {
        // Bật map hiện tại, tắt map khác
        for (int i = 0; i < maps.Length; i++)
            maps[i].mapRoot.SetActive(i == currentIndex);

        // Đặt spawn lần đầu (vào từ "phía trái" mặc định, tùy đại ca)
        if (maps[currentIndex].leftSpawn)
            player.position = maps[currentIndex].leftSpawn.position;

        var bg = maps[currentIndex].background;
        if (playerScript && bg) playerScript.SetBounds(bg);
        if (cameraFollow && bg) cameraFollow.SetBounds(bg);
    }

    // Gọi khi player đi ra khỏi current bằng cổng Left/Right
    public void Travel(GateSide exitSide)
    {
        int targetIndex = -1;
        Transform targetSpawn = null;

        if (exitSide == GateSide.Left)
        {
            targetIndex = maps[currentIndex].leftNeighbor;      // map đích ở bên trái
            if (targetIndex == -1) return;                      // không có map trái
            targetSpawn = maps[targetIndex].rightSpawn;         // vào từ phải của map đích
        }
        else // Right
        {
            targetIndex = maps[currentIndex].rightNeighbor;     // map đích ở bên phải
            if (targetIndex == -1) return;                      // không có map phải
            targetSpawn = maps[targetIndex].leftSpawn;          // vào từ trái của map đích
        }

        var bg = maps[targetIndex].background;
        if (playerScript && bg) playerScript.SetBounds(bg);
        if (cameraFollow && bg) cameraFollow.SetBounds(bg);
        print("Đã set target qua map moi"+targetIndex);

        // Tắt map cũ, bật map mới
        maps[currentIndex].mapRoot.SetActive(false);
        maps[targetIndex].mapRoot.SetActive(true);

        // Teleport player sang spawn của map đích
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb) { rb.linearVelocity = Vector2.zero; rb.position = targetSpawn.position; }
        else { player.position = targetSpawn.position; }
        // Cập nhật chỉ số hiện tại + bounds
        currentIndex = targetIndex;
    }

 
}