using UnityEngine;

public class SwordDamageDealer : MonoBehaviour
{
    // Không còn sát thương cố định ở đây
    private int currentWeaponDamage;
    private PlayerBase playerBase;

    // Hàm MỚI: WeaponManager sẽ gọi hàm này
    public void Initialize(int weaponDamage, PlayerBase player)
    {
        this.currentWeaponDamage = weaponDamage;
        this.playerBase = player;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Va chạm Trigger với: " + other.name);
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null && playerBase != null && playerBase.data != null)
        {
            // Dùng sát thương đã được "nạp"
            int totalDamage = this.currentWeaponDamage + playerBase.data.baseDamage;
            enemy.TakeDamage(totalDamage);
            Debug.Log($"Kiếm gây {totalDamage} sát thương lên {enemy.name}");
        }
    }
}