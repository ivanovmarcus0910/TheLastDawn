using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    // BỎ: public int damage = 25;
    // THÊM: Biến này sẽ lưu sát thương được truyền từ PlayerShooting
    private int totalDamageToDeal;

    // Hàm Setup MỚI: Nhận cả hướng VÀ sát thương
    public void Setup(Vector2 direction, int totalDamage)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;

        // Lưu lại sát thương được truyền vào
        this.totalDamageToDeal = totalDamage;

        // Lật sprite (giữ nguyên)
        if (direction.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        else transform.localScale = new Vector3(1, 1, 1);

        Destroy(gameObject, 1f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        EnemyBase enemy = hitInfo.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            // Gây sát thương bằng giá trị đã được truyền vào
            enemy.TakeDamage(totalDamageToDeal);
        }
        Destroy(gameObject);
    }
}