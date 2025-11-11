using UnityEngine;
using UnityEngine.SceneManagement;

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
    public void ChangeCurrentMap(int index)
    {
        currentIndex = index;
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

            targetIndex = maps[currentIndex].leftNeighbor;
            print($"Travel Left from {currentIndex} to {targetIndex}");

            if (targetIndex == -1) return;
            targetSpawn = maps[targetIndex].rightSpawn;
        }
        else
        {
            targetIndex = maps[currentIndex].rightNeighbor;
            print($"Travel Right from {currentIndex} to {targetIndex}");

            if (targetIndex == -1) return;
            if (targetIndex == 6)
            {
                ChuyenScene();
                return;
                Debug.Log("Bạn đã đến khu vực bí mật!");
                
            }
            targetSpawn = maps[targetIndex].leftSpawn;
        }

        // 1️⃣ Bật map mới trước
        maps[targetIndex].mapRoot.SetActive(true);

        maps[currentIndex].mapRoot.SetActive(false);

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.simulated = false;         // tắt vật lý 1 frame
            rb.linearVelocity = Vector2.zero;   // dừng mọi lực (an toàn hơn linearVelocity)
        }

        player.position = targetSpawn.position; // dịch thẳng qua vị trí mới

        if (rb)
        {
            rb.simulated = true;          // bật lại physics
        }

        // 3️⃣ Cập nhật bounds cho player & camera
        var bg = maps[targetIndex].background;
        if (playerScript && bg) playerScript.SetBounds(bg);
        if (cameraFollow && bg) cameraFollow.SetBounds(bg);

        // 4️⃣ Cập nhật index & input map
        currentIndex = targetIndex;
        var map = playerScript.inputActions.FindActionMap("Player", true);
        map.Enable();

        Debug.Log($"✅ Traveled to map: {maps[targetIndex].name}");
    }
    public void ChuyenScene()
    {
        SceneManager.LoadScene("SpaceStation");

    }


}