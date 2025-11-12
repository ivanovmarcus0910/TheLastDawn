using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    public RecylableInventoryManager inventoryManage;

    // Hàm thêm vật phẩm
    public void AddItem(ItemData item, int count)
    {
        if (item == null) return;

        if (inventoryManage == null)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            // Logic lưu trữ (Giữ nguyên code của bạn)
            if (inventoryManage.hasItem(item))
            {
                Debug.Log("Đã có item trong kho");
                inventoryManage.increaseQuantity(item); // Sẽ được gọi 'count' lần
            }
            else
            {
                Debug.Log("Chưa có item trong kho");
                inventoryManage.AddInventoryItem(item); // Sẽ được gọi 1 lần
            }
        }

        if (QuestManager.Instance != null)
        {
            
            QuestManager.Instance.UpdateQuestProgress(ObjectiveType.Collect, item.itemName, count);
        }
    }
}