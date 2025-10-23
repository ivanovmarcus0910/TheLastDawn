using UnityEngine;

public class SwordDamageDealer : MonoBehaviour
{
 
    public int swordDamage = 25;
    

    private PlayerBase playerBase; // Dùng để lấy sát thương cơ bản của Player

    void Awake()
    {
        // Tìm component PlayerBase ở GameObject cha gần nhất (Player)
        playerBase = GetComponentInParent<PlayerBase>();
        if (playerBase == null)
        {
            Debug.LogError("SwordDamageDealer không tìm thấy PlayerBase ở trên!");
        }
    }

    // Hàm này sẽ tự động được gọi khi Hitbox (là Trigger) va chạm với Collider khác
    void OnTriggerEnter2D(Collider2D other)
    {
       
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        if (enemy != null) 
        {
            // Tính toán tổng sát thương = Sát thương của kiếm + Sát thương cơ bản của Player
            int totalDamage = this.swordDamage;
            if (playerBase != null && playerBase.data != null)
            {
                totalDamage += playerBase.data.baseDamage;
            }

            // Gọi hàm TakeDamage của quái vật để gây sát thương
            enemy.TakeDamage(totalDamage);

            Debug.Log($"Kiếm gây {totalDamage} sát thương lên {enemy.name}"); // In ra để kiểm tra
        }
    }
}