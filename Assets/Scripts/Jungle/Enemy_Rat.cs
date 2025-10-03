using UnityEngine;
using System; // để dùng Action

public class Enemy_Rat : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private int startDirection = -1;
    [SerializeField] private Animator animator;

    [Header("Attack Zones")]
    public DetectionZone attackDetectionZone;   // Vùng phát hiện player
    public Transform attackZone;                // Collider attack (luôn bật)
    public Action onDeath;   // event để Spawner nghe khi con này chết

    private int currentDirection;
    private float halfWidth;
    private Vector2 movement;

    void Start()
    {
        // Lấy nửa chiều rộng sprite
        halfWidth = spriteRenderer.bounds.extents.x;
        currentDirection = startDirection;

        // Đặt hướng ban đầu
        spriteRenderer.flipX = (startDirection == -1);

        // Cập nhật vị trí các zone theo hướng ban đầu
        FlipChildZones();
    }

    void FixedUpdate()
    {
        if (attackDetectionZone == null || attackDetectionZone.detectedColliders.Count == 0)
        {
            // Enemy chạy patrol
            movement.x = speed * currentDirection;
            movement.y = rigidBody.linearVelocity.y; // giữ gravity
            rigidBody.linearVelocity = movement;

            PatrolCheck();
            animator.SetBool("HasTarget", false);
        }
        else
        {
            // Enemy phát hiện player → đứng lại, chơi animation Attack
            rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
            animator.SetBool("HasTarget", true);
        }
    }

    private void PatrolCheck()
    {
        Vector2 position = transform.position;
        Vector2 direction = (currentDirection == 1) ? Vector2.right : Vector2.left;

        // Raycast ngang check tường
        if (Physics2D.Raycast(position, direction, halfWidth + 0.1f, LayerMask.GetMask("Ground")))
        {
            Flip();
        }

        // Raycast xuống check mép platform
        Vector2 downCheckPos = position + new Vector2(currentDirection * halfWidth, 0f);
        if (!Physics2D.Raycast(downCheckPos, Vector2.down, 1f, LayerMask.GetMask("Ground")))
        {
            Flip();
        }

        // Debug rays
        Debug.DrawRay(position, direction * (halfWidth + 0.1f), Color.red);
        Debug.DrawRay(downCheckPos, Vector2.down * 1f, Color.yellow);
    }

    private void Flip()
    {
        currentDirection *= -1;

        // Flip sprite
        spriteRenderer.flipX = (currentDirection == -1);

        // Flip cả AttackZone và DetectionZone
        FlipChildZones();
    }

    private void FlipChildZones()
    {
        // Đảo AttackDetectionZone
        if (attackDetectionZone != null)
        {
            Vector3 pos = attackDetectionZone.transform.localPosition;
            pos.x = Mathf.Abs(pos.x) * currentDirection;
            attackDetectionZone.transform.localPosition = pos;
        }

        // Đảo Attack Collider
        if (attackZone != null)
        {
            Vector3 pos = attackZone.localPosition;
            pos.x = Mathf.Abs(pos.x) * currentDirection;
            attackZone.localPosition = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            onDeath?.Invoke();  // báo cho Spawner
            Destroy(gameObject);           // xóa enemy
        }
    }
}
