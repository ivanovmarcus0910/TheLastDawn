//using UnityEngine;
//using System;

//public class EnemyLootDropper : MonoBehaviour
//{
//    public LootTable lootTable; 
//    private EnemyBase enemyBase;
//    private string _enemyID;

//    void Start()
//    {
//        enemyBase = GetComponent<EnemyBase>();

//        if (enemyBase != null)
//        {
//            Debug.Log("✅ LOOT SETUP: Dang ky su kien cho " + gameObject.name);
//            enemyBase.onDeath += DropLoot;
//        }
//        else
//        {
//            Debug.LogError("❌ SETUP ERROR: Khong tim thay EnemyBase. Hay kiem tra Prefab.");
//        }
//    }

//    // Hàm này được gọi khi sự kiện onDeath xảy ra
//    public void DropLoot()
//    {
//        if (lootTable == null || lootTable.lootItems.Length == 0)
//        {
//            Debug.Log("⚠️ LOOT FAIL: LootTable NULL hoặc không có vật phẩm.");
//            if (enemyBase != null) enemyBase.onDeath -= DropLoot;
//            return;
//        }

//        Debug.Log("✅ LOOT CHECK: Bắt đầu chọn DUY NHẤT 1 vật phẩm.");

//        // 1. Tính tổng Trọng lượng
//        float totalWeight = 0f;
//        foreach (var loot in lootTable.lootItems)
//        {
//            totalWeight += loot.dropChance;
//        }

//        if (totalWeight <= 0) return; 

//        float randomPoint = UnityEngine.Random.value * totalWeight; 


//        foreach (var loot in lootTable.lootItems)
//        {
//            // Nếu điểm Random < Trọng lượng của vật phẩm hiện tại
//            if (randomPoint < loot.dropChance)
//            {
//                if (loot.item == null || loot.item.itemPrefab == null)
//                {
//                    randomPoint -= loot.dropChance;
//                    continue;
//                }

//                Vector3 dropPosition = transform.position 
//                    + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0.5f, 0f);

//                GameObject droppedObject = Instantiate(
//                    loot.item.itemPrefab,
//                    dropPosition,
//                    Quaternion.identity
//                );

//                // Gán ItemData và lực đẩy
//                ItemWorld itemWorld = droppedObject.GetComponent<ItemWorld>();
//                if (itemWorld != null)
//                {
//                    itemWorld.SetItemData(loot.item);
//                    itemWorld.quantity = 1;

//                    // Thêm lực đẩy nhẹ 
//                    Rigidbody2D itemRb = droppedObject.GetComponent<Rigidbody2D>();
//                    if (itemRb != null)
//                    {
//                        itemRb.AddForce(new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 1f), ForceMode2D.Impulse);
//                    }
//                }

//                Debug.Log($"✅ DROP SUCCESS: Da chon va tao ra {loot.item.itemName}.");

//                if (enemyBase != null)
//                {
//                    enemyBase.onDeath -= DropLoot;
//                }
//                return;
//            }

//            // Chuyển sang vật phẩm tiếp theo
//            randomPoint -= loot.dropChance;
//        }

//        if (enemyBase != null)
//        {
//            enemyBase.onDeath -= DropLoot;
//        }
//    }
//}

using UnityEngine;
using System;

public class EnemyLootDropper : MonoBehaviour
{
    public LootTable lootTable;
    private EnemyBase enemyBase;


    private string _enemyID;

    void Start()
    {
        enemyBase = GetComponent<EnemyBase>();

       
        if (enemyBase != null && enemyBase.data != null)
        {
           
            _enemyID = enemyBase.data.enemyName;

            Debug.Log("✅ LOOT SETUP: Dang ky su kien cho " + gameObject.name);

           
            enemyBase.onDeath += DropLoot;

           
            enemyBase.onDeath += ReportKillToQuestManager;
        }
        else if (enemyBase != null)
        {
           
            _enemyID = gameObject.name;
            Debug.Log("⚠️ LOOT SETUP: Khong tim thay EnemyData, dung ten GameObject lam ID.");
            enemyBase.onDeath += DropLoot;
            enemyBase.onDeath += ReportKillToQuestManager;
        }
        else
        {
            Debug.LogError("❌ SETUP ERROR: Khong tim thay EnemyBase. Hay kiem tra Prefab.");
        }
      
    }

    private void ReportKillToQuestManager()
    {
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(_enemyID))
        {
            QuestManager.Instance.UpdateQuestProgress(ObjectiveType.Kill, _enemyID, 1);
            Debug.Log($"✅ QUEST REPORT: Da bao cao tieu diet {_enemyID} cho Quest Manager.");
        }

        if (enemyBase != null)
        {
            enemyBase.onDeath -= ReportKillToQuestManager;
        }
    }

    public void DropLoot()
    {
   

        if (lootTable == null || lootTable.lootItems.Length == 0)
        {
            Debug.Log("⚠️ LOOT FAIL: LootTable NULL hoặc không có vật phẩm.");

    
            if (enemyBase != null) enemyBase.onDeath -= DropLoot;

            return;
        }

        Debug.Log("✅ LOOT CHECK: Bắt đầu chọn DUY NHẤT 1 vật phẩm.");

      
        float totalWeight = 0f;
        foreach (var loot in lootTable.lootItems)
        {
            totalWeight += loot.dropChance;
        }

        if (totalWeight <= 0)
        {
            if (enemyBase != null) enemyBase.onDeath -= DropLoot;
            return;
        }

        float randomPoint = UnityEngine.Random.value * totalWeight;


        foreach (var loot in lootTable.lootItems)
        {
  
            if (randomPoint < loot.dropChance)
            {
                if (loot.item == null || loot.item.itemPrefab == null)
                {
                    randomPoint -= loot.dropChance;
                    continue;
                }

                Vector3 dropPosition = transform.position
                    + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0.5f, 0f);

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

                    // Thêm lực đẩy nhẹ 
                    Rigidbody2D itemRb = droppedObject.GetComponent<Rigidbody2D>();
                    if (itemRb != null)
                    {
                        itemRb.AddForce(new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 1f), ForceMode2D.Impulse);
                    }
                }

                Debug.Log($"✅ DROP SUCCESS: Da chon va tao ra {loot.item.itemName}.");

                // Hủy đăng ký DropLoot sau khi hoàn thành
                if (enemyBase != null)
                {
                    enemyBase.onDeath -= DropLoot;
                }
                return;
            }

            // Chuyển sang vật phẩm tiếp theo
            randomPoint -= loot.dropChance;
        }

        // Hủy đăng ký DropLoot nếu không có vật phẩm nào rơi ra
        if (enemyBase != null)
        {
            enemyBase.onDeath -= DropLoot;
        }
    }
}