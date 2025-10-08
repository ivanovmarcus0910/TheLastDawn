using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Playermove2 : MonoBehaviour
{
    // ==== Input ====
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;

    // ==== Movement ====
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float jumpForce = 6f;
    private Rigidbody2D rb;
    private Animator anim;

    int jumpCount = 0;
    [SerializeField] int maxJumps = 2;

    // ==== Bounds ====
    [Header("Bounds by Background")]
    [SerializeField] SpriteRenderer background;   // gán ở Inspector hoặc gọi SetBounds(bg)
    float minX, maxX, minY, maxY;
    Vector2 halfSize; // half size của player (dùng collider)
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        RecalcHalfSize(); // tính trước để SetBounds dùng
    }

    void Start()
    {
        // Lấy action từ Input System (PlayerInput phải có Action Asset)
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        jumpAction.performed += _ => Jump();

        if (background != null)
            SetBounds(background);
    }

    void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
    }

    void Update()
    {
        // Đọc input (Update) -> áp dụng vật lý (FixedUpdate)
        moveInput = moveAction.ReadValue<Vector2>();

        // Flip nhân vật theo hướng chạy
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Sign(moveInput.x) * Mathf.Abs(s.x);
            transform.localScale = s;
        }

        // Animation
        bool isRunning = Mathf.Abs(moveInput.x) > 0.01f;
        if (anim) anim.SetBool("run", isRunning);
    }

    void FixedUpdate()
    {
        // chỉ điều khiển X, để Y cho physics
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // clamp: chỉ sửa khi vượt biên, KHÔNG MovePosition mỗi frame
        if (background != null)
        {
            var p = rb.position;
            float cx = Mathf.Clamp(p.x, minX, maxX);
            float cy = Mathf.Clamp(p.y, minY, maxY);
            if (cx != p.x || cy != p.y)
            {
                rb.position = new Vector2(cx, cy);
                if (cx != p.x) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // đập tường thì triệt tiêu X
                if (cy != p.y) rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // chạm sàn/trần thì triệt tiêu Y
            }
        }
    }

    void Jump()
    {
        if (jumpCount < maxJumps)
        {
            // reset vận tốc rơi để nhảy đều
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // chạm mặt đất (normal hướng lên) thì reset jump
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
            {
                jumpCount = 0;
                break;
            }
        }
    }

    // ==== Bounds API ====
    public void SetBounds(SpriteRenderer bg)
    {
        background = bg;
        Bounds b = bg.bounds;

        // cộng trừ halfSize để giữ player hoàn toàn trong khung
        minX = b.min.x + halfSize.x;
        maxX = b.max.x - halfSize.x;
        minY = b.min.y + halfSize.y;
        maxY = b.max.y - halfSize.y;

        // Nếu player đang ở ngoài, đưa về trong khung ngay
        Vector2 p = rb.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        rb.position = p;
    }

    void RecalcHalfSize()
    {
        if (col != null)
        {
            Bounds cb = col.bounds;
            halfSize = new Vector2(cb.extents.x, cb.extents.y);
        }
        else
        {
            // fallback: lấy theo sprite/scale (ít chính xác hơn)
            halfSize = GetComponent<SpriteRenderer>()
                ? (Vector2)GetComponent<SpriteRenderer>().bounds.extents
                : new Vector2(0.5f, 0.5f);
        }
    }

}
