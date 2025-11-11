using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Item", menuName = "Item/Create Equipment Item")]
public class ItemData_Equipment : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string itemName;             // Tên hiển thị
    public Sprite icon;                 // Ảnh trong UI
    public int price;                   // Giá bán/mua
    public GameObject prefab;           // Prefab trong game (nếu có)
    [TextArea] public string description;

    [Header("Loại trang bị")]
    public EquipmentSlot slotType;      // Vị trí mặc: Mũ, Giáp, Giày...

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
        return $"{itemName} | HP+{bonusHealth}, DEF+{bonusDefense}, SPD+{bonusMoveSpeed}, DMG+{bonusDamage}";
    }
}
