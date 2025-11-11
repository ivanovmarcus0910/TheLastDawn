using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Tham chiếu Hệ thống")]
    // Kéo EquipmentManager (trên Player) vào đây
    public EquipmentManager equipmentManager;
    // Kéo PlayerBase (trên Player) vào đây
    public PlayerBase playerBase;

    [Header("Vị trí Gắn Vũ khí")]
    // Kéo GameObject rỗng 'WeaponHoldPoint' vào đây
    public Transform weaponHoldPoint;

    // Biến nội bộ để lưu trữ vũ khí
    private GameObject currentWeaponObject; // Vũ khí đang cầm trên tay
    private PlayerShooting currentShootingScript;
    private PlayerSwordAttack currentSwordAttackScript;
    private WeaponData currentWeaponData; // Lưu data của vũ khí đang cầm

    private bool isSlot1Active = true; // Bắt đầu bằng vũ khí ở slot 1
    private float nextFireTime = 0f;

    void Start()
    {
        // Tự động lấy component nếu chưa gán
        if (equipmentManager == null)
            equipmentManager = GetComponent<EquipmentManager>();
        if (playerBase == null)
            playerBase = GetComponent<PlayerBase>();

        // Đăng ký lắng nghe sự kiện: Khi nào trang bị thay đổi, gọi hàm UpdateEquippedWeapon()
        if (equipmentManager != null)
        {
            equipmentManager.OnEquipmentChanged += UpdateEquippedWeapon;
            // Cập nhật vũ khí ngay khi bắt đầu game
            UpdateEquippedWeapon();
        }
        else
        {
            Debug.LogError("WeaponManager không tìm thấy EquipmentManager!");
        }
    }

    // Hủy đăng ký khi đối tượng bị hủy
    void OnDestroy()
    {
        if (equipmentManager != null)
            equipmentManager.OnEquipmentChanged -= UpdateEquippedWeapon;
    }

    // Hàm được gọi mỗi khi trang bị thay đổi
    void UpdateEquippedWeapon()
    {
        // Lấy data của vũ khí đang active (Slot 1 hoặc 2)
        WeaponData dataToEquip = isSlot1Active ? equipmentManager.WeaponSlot1 : equipmentManager.WeaponSlot2;

        // Gọi hàm con để trang bị
        EquipWeapon(dataToEquip);
    }

    // Hàm con để tạo/hủy vũ khí
    // Hàm con để tạo/hủy vũ khí (ĐÃ SỬA LỖI)
    void EquipWeapon(WeaponData data)
    {
        // 1. Hủy vũ khí cũ
        if (currentWeaponObject != null) Destroy(currentWeaponObject);
        currentShootingScript = null;
        currentSwordAttackScript = null;
        currentWeaponData = null;

        // 2. Nếu slot không có gì (data = null), thì dừng lại
        if (data == null)
        {
            Debug.Log("Slot vũ khí đang trống.");
            return;
        }

        // 3. Lưu data và tạo vũ khí mới từ Prefab
        currentWeaponData = data;
        currentWeaponObject = Instantiate(data.weaponPrefab, weaponHoldPoint);

        // 4. Lấy script và "nạp" thông tin
        if (data.weaponType == WeaponType.Gun)
        {
            currentShootingScript = currentWeaponObject.GetComponentInChildren<PlayerShooting>();
            if (currentShootingScript != null)
                currentShootingScript.Initialize(data, playerBase);
            else
                Debug.LogError("Prefab súng thiếu script PlayerShooting!");
        }
        else if (data.weaponType == WeaponType.Sword)
        {
            currentSwordAttackScript = currentWeaponObject.GetComponentInChildren<PlayerSwordAttack>();
            if (currentSwordAttackScript != null)
            {
                // --- DÒNG SỬA LỖI QUAN TRỌNG ---
                // Tìm SwordDamageDealer từ GameObject vũ khí, không phải từ script khác
                SwordDamageDealer damageDealer = currentWeaponObject.GetComponentInChildren<SwordDamageDealer>();
                // ---------------------------------

                if (damageDealer != null)
                    damageDealer.Initialize(data.weaponDamage, playerBase);
                else
                    Debug.LogError("Prefab kiếm (SwordSprite) thiếu script SwordDamageDealer!");
            }
            else
                Debug.LogError("Prefab kiếm (SwordPivot) thiếu script PlayerSwordAttack!");
        }
    }

    // Hàm Update chính để nhận Input
    void Update()
    {
        // 1. ĐỔI VŨ KHÍ (Phím G)
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            // Kiểm tra xem có 2 vũ khí để đổi không
            if (equipmentManager.WeaponSlot1 != null && equipmentManager.WeaponSlot2 != null)
            {
                isSlot1Active = !isSlot1Active; // Đổi qua lại giữa true và false
                UpdateEquippedWeapon(); // Cập nhật lại vũ khí trên tay
            }
        }

        // 2. TẤN CÔNG (Chuột trái)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Nếu đang cầm súng (có thể là slot 1 hoặc 2)
            if (currentShootingScript != null)
            {
                if (Time.time >= nextFireTime)
                {
                    currentShootingScript.PerformShoot();
                    if (currentWeaponData.fireRate > 0)
                        nextFireTime = Time.time + 1f / currentWeaponData.fireRate;
                }
            }
            // Nếu đang cầm kiếm (có thể là slot 1 hoặc 2)
            else if (currentSwordAttackScript != null)
            {
                currentSwordAttackScript.PerformAttack();
            }
        }
    }
}