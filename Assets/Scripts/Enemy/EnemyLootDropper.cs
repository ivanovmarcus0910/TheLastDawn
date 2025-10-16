using UnityEngine;
using System;

public class EnemyLootDropper : MonoBehaviour
{
    public LootTable lootTable; // LootTable cho loại quái vật này

    private EnemyBase enemyBase;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();

        if (enemyBase != null)
        {
            // Thêm Log để xác nhận việc đăng ký đã xảy ra
            //Debug.Log("✅ LOOT SETUP: Dang ky su kien cho " + gameObject.name);
            enemyBase.onDeath += DropLoot;
        }
        else
        {
            // Thêm Log lỗi nếu không tìm thấy EnemyBase
            Debug.LogError("❌ SETUP ERROR: Khong tim thay EnemyBase. Hay kiem tra Prefab.");
        }
    }

    // Hàm này được gọi khi sự kiện onDeath xảy ra
    public void DropLoot()
    {
        if (lootTable == null || lootTable.lootItems.Length == 0) return;

        foreach (var loot in lootTable.lootItems)
        {
            float roll = UnityEngine.Random.value;

            if (roll <= loot.dropChance)
            {

                if (loot.item == null || loot.item.itemPrefab == null)
                {
                    //Debug.LogError("❌ PREFAB ERROR: Item Prefab cua " + loot.item.itemName + " dang la NULL.");
                    continue; // Khong tao vat pham neu no la NULL
                }

                // Tính vị trí rớt ngẫu nhiên (Giữ nguyên)
                Vector3 dropPos = transform.position
                        + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f),
                                      0.5f,
                                      0f);

                // Tạo đối tượng vật phẩm
                GameObject droppedObject = Instantiate(
                    loot.item.itemPrefab,
                    dropPos,
                    Quaternion.identity
                );

                // 📢 KIỂM TRA QUAN TRỌNG NHẤT: VẬT PHẨM ĐÃ ĐƯỢC TẠO CHƯA?
                //Debug.Log("✅ INSTANTIATED: Da tao vat pham tai Z=" + droppedObject.transform.position.z + ".");

                // Gán ItemData vào script ItemWorld
                ItemWorld itemWorld = droppedObject.GetComponent<ItemWorld>();
                if (itemWorld != null)
                {
                    itemWorld.itemData = loot.item;
                    itemWorld.quantity = 1;

                    // Thêm lực đẩy nhẹ
                    Rigidbody2D itemRb = droppedObject.GetComponent<Rigidbody2D>();
                    if (itemRb != null)
                    {
                        itemRb.AddForce(new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 1f), ForceMode2D.Impulse);
                    }
                }
            }
        }

        // Ngừng lắng nghe sự kiện
        if (enemyBase != null)
        {
            enemyBase.onDeath -= DropLoot;
        }
    }
}