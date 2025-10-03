using UnityEngine;
using System;

[System.Serializable]
public class SpawnSlot
{
    public Transform point;          // Vị trí spawn
    public GameObject prefab;        // Loại quái của slot này
    public float respawnDelay = 5f;  // Chờ bao lâu sau khi chết thì spawn lại
}

public class EnemySpawnerPerSlot : MonoBehaviour
{
    [Header("Per-Slot Config")]
    public SpawnSlot[] slots;        // Khai báo từng slot trong Inspector

    private GameObject[] currentEnemies;
    private float[] respawnTimers;

    void Start()
    {
        currentEnemies = new GameObject[slots.Length];
        respawnTimers = new float[slots.Length];

        // Spawn ngay các slot đang trống khi bắt đầu (nếu muốn)
        for (int i = 0; i < slots.Length; i++)
            TrySpawnAt(i);
    }

    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (currentEnemies[i] != null) continue;

            if (respawnTimers[i] > 0f)
            {
                respawnTimers[i] -= Time.deltaTime;
                continue;
            }

            TrySpawnAt(i);
        }
    }

    void TrySpawnAt(int index)
    {
        var slot = slots[index];
        if (slot.point == null || slot.prefab == null) return;
        if (currentEnemies[index] != null) return;

        var enemy = Instantiate(slot.prefab, slot.point.position, Quaternion.identity);
        currentEnemies[index] = enemy;

        // Gắn sự kiện chết cho những script quái khác nhau
        // (thêm else-if nếu có loại mới)
        if (!BindDeath(enemy, index))
        {
            Debug.LogWarning($"[Spawner] Prefab {enemy.name} không có onDeath. " +
                             "Hãy gọi onDeath trong enemy khi chết để spawner respawn.");
        }
    }

    bool BindDeath(GameObject enemy, int slotIndex)
    {
        // 1) Enemy_Rat
        var rat = enemy.GetComponent<Enemy_Rat>();
        if (rat != null)
        {
            rat.onDeath += () => OnEnemyDeath(slotIndex);
            return true;
        }

        // 2) Enemy_Bat (nếu có)
        var bat = enemy.GetComponent<Enemy_Fly>();
        if (bat != null)
        {
            bat.onDeath += () => OnEnemyDeath(slotIndex);
            return true;
        }

        // 3) (Tùy chọn) Nếu đại ca có interface chung IEnemyDeath { event Action onDeath; }
        // var any = enemy.GetComponent<IEnemyDeath>();
        // if (any != null) { any.onDeath += () => OnEnemyDeath(slotIndex); return true; }

        return false;
    }

    void OnEnemyDeath(int slotIndex)
    {
        currentEnemies[slotIndex] = null;
        respawnTimers[slotIndex] = slots[slotIndex].respawnDelay; // chờ đúng delay của slot
    }
}
