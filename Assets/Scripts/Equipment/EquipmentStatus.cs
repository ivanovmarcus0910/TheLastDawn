using System;
using System.Collections.Generic;

[Serializable]   // 1
public class EquipmentStatus   // 2
{
    public List<int> slots;    // 3

    public EquipmentStatus()   // 4
    {
        slots = new List<int> { 1, 1, 1, 1, 1, 1, 1 }; // 5
    }

    public void SetSlot(int index, bool isEquipped)
    {
        // 1️⃣ Kiểm tra nếu chỉ số slot nằm ngoài giới hạn danh sách
        if (index < 0 || index >= slots.Count)
            return;

        // 2️⃣ Nếu isEquipped = true → đang mặc
        if (isEquipped)
        {
            slots[index] = 1;
        }
        // 3️⃣ Ngược lại → tháo ra
        else
        {
            slots[index] = 0;
        }
    }

    public void SetSlot(EquipmentSlot slot, bool isEquipped) // 9
    {
        SetSlot((int)slot, isEquipped);                 // 10
    }

    public bool IsEquipped(EquipmentSlot slot)          // 11
    {
        return slots[(int)slot] == 1;                   // 12
    }

    public override string ToString()                   // 13
    {
        return $"[Trang bị] {string.Join(", ", slots)}"; // 14
    }
}
