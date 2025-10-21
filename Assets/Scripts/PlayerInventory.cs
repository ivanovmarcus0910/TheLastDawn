// PlayerInventory.cs
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Dictionary lưu trữ kho đồ
    [SerializeField]
    public RecylableInventoryManager inventoryManage;
    // Hàm thêm vật phẩm
    public void AddItem(ItemData item, int count)
    {
        if (item == null) return;

        if (inventoryManage.hasItem(item))
        {
            inventoryManage.increaseQuantity(item); // Tăng số lượng
        }
        else
        {
            inventoryManage.AddInventoryItem(item); // Thêm mới
        }

        //Debug.Log("Đã nhặt: " + count + " x " + item.itemName + ". Tổng cộng: " + inventory[item]);
    }
}