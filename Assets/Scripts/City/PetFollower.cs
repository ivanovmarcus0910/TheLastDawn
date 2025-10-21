using UnityEngine;

public class PetFollower : MonoBehaviour
{
    public Transform target;             // Player cần theo
    public float followDistance = 0.3f;  // Khoảng cách dừng lại
    public float moveSpeed = 2f;         // Tốc độ di chuyển

    private Animator anim;
    private SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (target == null) return;

        // Tính khoảng cách giữa chó và player
        float distance = Vector2.Distance(transform.position, target.position);

        // Nếu xa hơn followDistance -> di chuyển về phía player
        if (distance > followDistance)
        {
            // Hướng di chuyển (từ chó → player)
            Vector3 direction = (target.position - transform.position).normalized;

            // Di chuyển theo hướng đó
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Lật sprite theo hướng di chuyển, không đổi scale
            if (sr != null)
                sr.flipX = direction.x < 0;

            // Bật animation đi bộ
            anim?.SetBool("isWalking", true);
        }
        else
        {
            // Khi đủ gần -> dừng
            anim?.SetBool("isWalking", false);
        }
    }
}
