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

        // Logic lưu trữ 
        if (inventoryManage.hasItem(item))
        {
            inventoryManage.increaseQuantity(item); // Tăng số lượng
        }
        else
        {
            inventoryManage.AddInventoryItem(item); // Thêm mới
        }

        GameEventHub.ItemCollected(item.name, Mathf.Max(1, count));
    }
}