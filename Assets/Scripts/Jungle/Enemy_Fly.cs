using UnityEngine;

public class Enemy_Fly : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed = 3f;
    [SerializeField] private int startDirection = -1;
    [SerializeField] private Animator animator;

    public DetectionZone attackDetectionZone;

    private int currentDirection;
    private float halfWidth;
    private Vector2 movement;

    void Start()
    {
        halfWidth = spriteRenderer.bounds.extents.x;
        currentDirection = startDirection;
        spriteRenderer.flipX = startDirection == -1 ? false : true;
    }

    void FixedUpdate()
    {
        if (attackDetectionZone.detectedColliders.Count == 0)
        {
            // Enemy bay bình thường
            movement.x = speed * currentDirection;
            movement.y = rigidBody.linearVelocity.y;
            rigidBody.linearVelocity = movement;

            setDirection();
            animator.SetBool("HasTarget", false);
        }
        else
        {
            // Enemy thấy player → dừng lại và chơi animation Attack
            animator.SetBool("HasTarget", true);
        }
    }

    private void setDirection()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Ground"))
            && rigidBody.linearVelocity.x > 0)
        {
            currentDirection *= -1;
            spriteRenderer.flipX = false;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Ground"))
            && rigidBody.linearVelocity.x < 0)
        {
            currentDirection *= -1;
            spriteRenderer.flipX = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(gameObject);           // xóa enemy
        }
    }
}
