using UnityEngine;

public class HandProjectile : MonoBehaviour
{
    public float speed = 10f; // Tốc độ bay
    public int damage = 10;   // Sát thương gây ra
    public float lifetime = 3f; // Thời gian tồn tại tối đa

    void Start()
    {
        // Tự hủy sau một thời gian để tránh lãng phí bộ nhớ
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Di chuyển tay về phía trước (Giả sử tay ban đầu đã quay đúng hướng)
        // Nếu bạn muốn tay luôn bay sang phải, dùng: transform.Translate(Vector3.right * speed * Time.deltaTime);
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    // Xử lý va chạm
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là kẻ địch không
        if (other.CompareTag("Player"))
        {
            // Thực hiện sát thương lên kẻ địch (cần script EnemyHealth)
            // Example: other.GetComponent<EnemyHealth>().TakeDamage(damage);

            // Tạo hiệu ứng va chạm (nếu có)

            // Hủy đối tượng tay sau khi va chạm
            Destroy(gameObject);
        }

        // Hoặc hủy khi chạm đất/tường (nếu cần)
        // if (other.CompareTag("Wall")) 
        // {
        //     Destroy(gameObject);
        // }
    }
}
