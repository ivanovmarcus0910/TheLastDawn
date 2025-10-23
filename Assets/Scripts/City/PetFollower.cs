using UnityEngine;

public class PetFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;             // Player
    public float followDistance = 0.4f;  // Khoảng cách lý tưởng
    public float moveSpeed = 2.5f;       // Tốc độ di chuyển
    public float jumpForce = 2f;         // Lực nhảy vật cản thấp
    public float teleportDistance = 6f;  // Khoảng cách tối đa để teleport
    public Transform teleportPoint;      // Điểm teleport (thường dưới chân player)

    [Header("Detection")]
    public float groundCheckDistance = 0.2f;
    public float obstacleCheckDistance = 0.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private bool isGrounded;
    private int groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // ✅ Tự động lấy layer Ground theo tên (khỏi cần gán thủ công)
        groundLayer = LayerMask.GetMask("Ground");

        if (groundLayer == 0)
            Debug.LogWarning("⚠️ Không tìm thấy layer 'Ground'. Hãy kiểm tra layer name trong Unity!");
    }

    void Update()
    {
        if (target == null) return;

        CheckGrounded();

        float distance = Vector2.Distance(transform.position, target.position);

        // 🌉 TELEPORT nếu quá xa hoặc rơi khỏi mặt đất quá lâu
        if (distance > teleportDistance)
        {
            TeleportToPlayer();
            return;
        }

        // 🐾 Nếu xa hơn followDistance thì tiến về Player
        if (distance > followDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMoving();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        // Di chuyển mượt
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        // Lật sprite theo hướng di chuyển
        if (sr != null)
            sr.flipX = direction.x < 0;

        anim?.SetBool("isWalking", true);

        // 🔍 Kiểm tra vật cản nhỏ phía trước
        RaycastHit2D obstacle = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(direction.x), obstacleCheckDistance, groundLayer);
        if (obstacle.collider != null && isGrounded)
        {
            Jump();
        }
    }

    void StopMoving()
    {
        anim?.SetBool("isWalking", false);
    }

    void Jump()
    {
        if (isGrounded && rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //anim?.SetTrigger("Jump");
        }
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }

    void TeleportToPlayer()
    {
        Debug.Log("🐾 Pet teleporting to player...");

        Vector3 newPos = teleportPoint != null
            ? teleportPoint.position
            : target.position + Vector3.down * 0.5f;

        transform.position = newPos;
        rb.linearVelocity = Vector2.zero;
        anim?.SetTrigger("Teleport");
    }

    private void OnDrawGizmosSelected()
    {
        // Debug ray hiển thị trong editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * obstacleCheckDistance);
    }
}
