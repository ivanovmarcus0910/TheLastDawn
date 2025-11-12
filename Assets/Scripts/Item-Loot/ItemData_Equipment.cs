using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Item", menuName = "Item/Create Equipment Item")]
public class ItemData_Equipment : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public ItemData baseData;   // Thông tin chung của item

    [Header("Chỉ số cộng thêm khi trang bị")]
    public int bonusHealth;             // +Máu
    public int bonusDefense;            // +Giáp
    public float bonusMoveSpeed;        // +Tốc độ di chuyển
    public float bonusJumpForce;        // +Độ nhảy
    public int bonusDamage;             // +Sát thương

    [Header("Tùy chọn khác")]
    public bool canBeSold = true;       // Có thể bán được không

    public override string ToString()
    {
        return $"{baseData.itemName} | HP+{bonusHealth}, DEF+{bonusDefense}, SPD+{bonusMoveSpeed}, DMG+{bonusDamage}";
    }
}
