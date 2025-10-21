// File: WeaponManager.cs
using UnityEngine;
using UnityEngine.InputSystem; // Thêm dòng này để dùng Input System mới

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon GameObjects")]
    public GameObject gunObject;
    public GameObject swordObject;

    [Header("Weapon Scripts")]
    // Sửa lại đúng kiểu dữ liệu của script
    public PlayerShooting playerShootingScript;
    public PlayerSwordAttack playerSwordAttackScript;

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

        // 2. TẤN CÔNG (Dùng Input System mới)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isGunEquipped)
            {
                playerShootingScript.PerformShoot(); // Tên hàm đã khớp
            }
            else
            {
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