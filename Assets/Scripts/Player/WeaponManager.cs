
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon GameObjects")]
    public GameObject gunObject;
    public GameObject swordObject;

    [Header("Weapon Scripts")]
   
    public PlayerShooting playerShootingScript;
    public PlayerSwordAttack playerSwordAttackScript;

    [Header("Shooting Settings")]
    public float fireRate = 5f; // Số viên đạn bắn ra mỗi giây (1f = 1 viên/giây)
    private float nextFireTime = 0f; // Thời điểm được phép bắn viên tiếp theo
    private bool isGunEquipped = true;

    void Start()
    {
        SwitchToGun();
    }

    void Update()
    {
        // 1. ĐỔI VŨ KHÍ (Dùng Input System mới)
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            if (isGunEquipped)
                SwitchToSword();
            else
                SwitchToGun();
        }

        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isGunEquipped)
            {
                // --- THÊM ĐIỀU KIỆN KIỂM TRA COOLDOWN ---
                if (Time.time >= nextFireTime)
                {
                    // Đã đến lúc được bắn
                    playerShootingScript.PerformShoot(); // Bắn

                    // Đặt lại thời gian hồi chiêu cho lần bắn kế tiếp
                    nextFireTime = Time.time + 1f / fireRate;
                }
                // ----------------------------------------
            }
            else
            {
                // Logic chém kiếm không cần cooldown ở đây (có thể thêm nếu muốn)
                playerSwordAttackScript.PerformAttack();
            }
        }
    }

    void SwitchToGun()
    {
        gunObject.SetActive(true);
        swordObject.SetActive(false);
        isGunEquipped = true;
    }

    void SwitchToSword()
    {
        gunObject.SetActive(false);
        swordObject.SetActive(true);
        isGunEquipped = false;
    }
}