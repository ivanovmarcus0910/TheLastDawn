using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    [Header("Cấu hình Scene")]
    public CameraScript cameraScript;     
    public SpriteRenderer stationBackground;
    public Transform spawnPoint;          

    void Start()
    {
        PlayerBase player = PlayerBase.Instance;

        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb) rb.simulated = false;

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

            if (cameraScript != null)
            {
                cameraScript.target = player.GetComponent<Transform>();

                cameraScript.SetBounds(stationBackground);
                player.SetBounds(stationBackground);
            }
        }
    }
}