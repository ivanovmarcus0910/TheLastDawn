using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

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

        Destroy(gameObject, 2f); // tự hủy sau 2s
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        print("Huy : "+hitInfo.name);
        // Hủy khi va chạm với vật thể khác
        Destroy(gameObject);
    }
}
