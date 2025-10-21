using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    public int damage = 25;
    // Hàm setup để nhận hướng bắn từ Player
    public void Setup(Vector2 direction)
    {
        print("Tao : "+direction);

        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;

        // 👉 Lật sprite theo hướng bay
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        Destroy(gameObject, 1f); // tự hủy sau 2s
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        print("Huy : "+hitInfo.name);
        EnemyBase enemy = hitInfo.GetComponent<EnemyBase>();

        // 2. Nếu đúng là quái vật (enemy != null)
        if (enemy != null)
        {
            // 3. Gọi hàm TakeDamage của nó và truyền sát thương vào
            enemy.TakeDamage(damage);
        }

        // 4. Luôn luôn hủy viên đạn sau khi va chạm với bất cứ thứ gì
        Destroy(gameObject);
    }
}
