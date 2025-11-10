using UnityEngine;
using System; // Cần thiết để sử dụng 'Action' (Sự kiện)

public class EquipmentManager : MonoBehaviour
{
    // Singleton để các script khác (như WeaponManager) có thể dễ dàng tìm thấy
    public static EquipmentManager Instance;

    [Header("Trang bị Hiện tại (Kéo Data vào đây để test)")]
    // 2 slot vũ khí chung, có thể là 2 súng hoặc 2 kiếm
    public WeaponData WeaponSlot1;
    public WeaponData WeaponSlot2;

    // Sự kiện (event) để thông báo cho WeaponManager biết khi trang bị thay đổi
    // (Ví dụ: khi bạn kéo thả đồ trong UI)
    public event Action OnEquipmentChanged;

    void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null)
        {
            Instance = this;
            // (Tùy chọn) Nếu bạn muốn Manager này tồn tại qua các màn chơi:
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // === CÁC HÀM NÀY SẼ ĐƯỢC UI PANEL GỌI SAU NÀY ===

    // Hàm này sẽ được UI Panel gọi (ví dụ: "Trang bị vào Slot 1")
    // slotIndex phải là 1 hoặc 2
    public void EquipWeapon(WeaponData newWeapon, int slotIndex)
    {
        if (slotIndex == 1)
        {
            WeaponSlot1 = newWeapon;
        }
        else if (slotIndex == 2)
        {
            WeaponSlot2 = newWeapon;
        }

        Debug.Log($"Đã trang bị {newWeapon.weaponName} vào Slot {slotIndex}");

        // Thông báo cho mọi script đang lắng nghe (như WeaponManager)
        OnEquipmentChanged?.Invoke();
    }

    // Hàm tháo vũ khí (ví dụ: "Tháo vũ khí ở Slot 1")
    public void UnequipWeapon(int slotIndex)
    {
        WeaponData unequippedWeapon = null;
        if (slotIndex == 1)
        {
            unequippedWeapon = WeaponSlot1;
            WeaponSlot1 = null;
        }
        else if (slotIndex == 2)
        {
            unequippedWeapon = WeaponSlot2;
            WeaponSlot2 = null;
        }

        if (unequippedWeapon != null)
            Debug.Log($"Đã tháo {unequippedWeapon.weaponName} khỏi Slot {slotIndex}");

        // Thông báo thay đổi
        OnEquipmentChanged?.Invoke();
    }
}