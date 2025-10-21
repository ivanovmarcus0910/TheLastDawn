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
            Debug.Log("✅ LOOT SETUP: Dang ky su kien cho " + gameObject.name);
            enemyBase.onDeath += DropLoot; // Đăng ký sự kiện
        }
        else
        {
            Debug.LogError("❌ SETUP ERROR: Khong tim thay EnemyBase. Hay kiem tra Prefab.");
        }
    }

    // Hàm này được gọi khi sự kiện onDeath xảy ra
    public void DropLoot()
    {
        if (lootTable == null || lootTable.lootItems.Length == 0)
        {
            Debug.Log("⚠️ LOOT FAIL: LootTable NULL hoặc không có vật phẩm.");
            // Ngừng lắng nghe sự kiện ngay cả khi không drop
            if (enemyBase != null) enemyBase.onDeath -= DropLoot;
            return;
        }

        Debug.Log("✅ LOOT CHECK: Bắt đầu chọn DUY NHẤT 1 vật phẩm.");

        // 1. Tính tổng Trọng lượng (Total Weight)
        float totalWeight = 0f;
        foreach (var loot in lootTable.lootItems)
        {
            totalWeight += loot.dropChance;
        }

        if (totalWeight <= 0) return; 

        // 2. Quay số Random (Roll the dice)
        float randomPoint = UnityEngine.Random.value * totalWeight; 

        // 3. Chọn Vật phẩm duy nhất (Single Weighted Selection)
        foreach (var loot in lootTable.lootItems)
        {
            // Nếu điểm Random < Trọng lượng của vật phẩm hiện tại, chọn nó
            if (randomPoint < loot.dropChance)
            {
                // Kiểm tra lỗi Prefab NULL
                if (loot.item == null || loot.item.itemPrefab == null)
                {
                    Debug.LogError("❌ PREFAB ERROR: Item Prefab cua " + loot.item.itemName + " dang la NULL.");
                    // Thay vì return, tiếp tục vòng lặp để kiểm tra các vật phẩm khác
                    randomPoint -= loot.dropChance;
                    continue;
                }
                
                // Xac dinh vi tri tao ra vat pham
                Vector3 dropPosition = transform.position 
                    + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0.5f, 0f);

                // Tạo đối tượng vật phẩm
                GameObject droppedObject = Instantiate(
                    loot.item.itemPrefab,
                    dropPosition,
                    Quaternion.identity
                );
                
                // Gán ItemData và lực đẩy
                ItemWorld itemWorld = droppedObject.GetComponent<ItemWorld>();
                if (itemWorld != null)
                {
                    itemWorld.SetItemData(loot.item);
                    itemWorld.quantity = 1;
                    
                    // Thêm lực đẩy nhẹ (đã sửa lỗi cú pháp)
                    Rigidbody2D itemRb = droppedObject.GetComponent<Rigidbody2D>();
                    if (itemRb != null)
                    {
                        itemRb.AddForce(new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 1f), ForceMode2D.Impulse);
                    }
                }
                
                // Log và Thoát - CHỈ RỚT 1 VẬT PHẨM
                Debug.Log($"✅ DROP SUCCESS: Da chon va tao ra {loot.item.itemName}.");
                
                // Ngừng lắng nghe sự kiện
                if (enemyBase != null)
                {
                    enemyBase.onDeath -= DropLoot;
                }
                return; // Thoát khỏi hàm
            }

            // Chuyển sang vật phẩm tiếp theo
            randomPoint -= loot.dropChance;
        }

        // Log nếu lỗi random hiếm gặp xảy ra (Không rớt gì cả)
        Debug.Log("⚠️ LOOT FAIL: Random Roll khong ra vat pham nao trong lan nay.");
        
        // Ngừng lắng nghe sự kiện
        if (enemyBase != null)
        {
            enemyBase.onDeath -= DropLoot;
        }
    }
}