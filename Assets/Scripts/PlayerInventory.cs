// PlayerInventory.cs
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Dictionary lưu trữ kho đồ
    public Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    // Hàm thêm vật phẩm
    public void AddItem(ItemData item, int count)
    {
        if (item == null) return;

        if (inventory.ContainsKey(item))
        {
            inventory[item] += count; // Tăng số lượng
        }
        else
        {
            inventory.Add(item, count); // Thêm mới
        }

        //Debug.Log("Đã nhặt: " + count + " x " + item.itemName + ". Tổng cộng: " + inventory[item]);
    }
}