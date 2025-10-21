using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBase : MonoBehaviour
{
    [Header("Hồ sơ Nhân vật")]
    public PlayerData data; // Kéo file PlayerData.asset vào đây

    [Header("UI")]
    public HealthBar playerHealthBar;
    public HealthBar playerManaBar;

    // === CÁC BIẾN NỘI BỘ ===
    // Từ PlayerHealth
    private int currentHealth;
    private int currentMana;

    // Từ Playermove2
    private Rigidbody2D rb;
    private Animator anim;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;
    private int jumpCount = 0;

    // --- BIẾN CHO BOUNDS (MỚI THÊM) ---
    private Collider2D col; // Cần Collider để tính kích thước
    private SpriteRenderer backgroundForBounds; // Để lưu background hiện tại
    private float minX, maxX, minY, maxY; // Tọa độ biên giới
    private Vector2 halfSize; // Nửa kích thước Player

    // === KHỞI TẠO ===
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>(); // <-- LẤY COLLIDER
        RecalcHalfSize();             // <-- TÍNH KÍCH THƯỚC BAN ĐẦU
    }

    void Start()
    {
        // Đọc chỉ số từ file Data
        if (data == null)
        {
            Debug.LogError("Chưa gán PlayerData cho PlayerBase!");
            return; // Ngăn lỗi nếu data chưa được gán
        }
        currentHealth = data.maxHealth;
        currentMana = data.maxMana;

        // Cập nhật UI
        if (playerHealthBar != null)
            playerHealthBar.UpdateBar(currentHealth, data.maxHealth);
        if (playerManaBar != null)
            playerManaBar.UpdateBar(currentMana, data.maxMana);

        // Thiết lập Input
        // Đảm bảo Input Actions đã được thiết lập trong Project Settings -> Input System Package
        try
        {
            moveAction = InputSystem.actions.FindAction("Move");
            jumpAction = InputSystem.actions.FindAction("Jump");
            jumpAction.performed += _ => Jump();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi khi tìm Input Action: {e.Message}. Hãy kiểm tra Input Action Asset của bạn.");
            this.enabled = false; // Tắt script nếu không có input
            return;
        }


    }

    // === KÍCH HOẠT INPUT ===
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

    // === XỬ LÝ HÀNG FRAME ===
    void Update()
    {
        // Đọc input di chuyển
        if (moveAction != null)
            moveInput = moveAction.ReadValue<Vector2>();
        else
            moveInput = Vector2.zero; // Không có input thì đứng yên

        // Lật nhân vật (Flip)
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            Vector3 s = transform.localScale;
            // Đảm bảo không lật ngược nếu scale gốc đã âm
            s.x = Mathf.Sign(moveInput.x) * Mathf.Abs(s.x);
            transform.localScale = s;
        }

        // Cập nhật Animation
        bool isRunning = Mathf.Abs(moveInput.x) > 0.01f;
        // Kiểm tra xem có Animator không trước khi gọi
        if (anim != null) anim.SetBool("run", isRunning);
    }

    void FixedUpdate()
    {
        // Áp dụng di chuyển vật lý
        // Đọc tốc độ di chuyển từ file Data (kiểm tra data null)
        float currentMoveSpeed = (data != null) ? data.moveSpeed : 0f;
        rb.linearVelocity = new Vector2(moveInput.x * currentMoveSpeed, rb.linearVelocity.y);

        // Gọi hàm kiểm tra và giới hạn vị trí
        ClampPositionToBounds(); // <-- GỌI HÀM GIỚI HẠN Ở ĐÂY
    }

    // === HÀNH ĐỘNG: NHẢY ===
    void Jump()
    {
        // Kiểm tra data null trước khi dùng jumpForce
        if (data != null && jumpCount < 2) // Giả sử maxJumps = 2
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * data.jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset nhảy
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
            {
                jumpCount = 0;
                break;
            }
        }
    }

    // === HỆ THỐNG MÁU ===
    public void TakeDamage(int damageAmount)
    {
        if (data == null) return; // Không có data thì không có máu

        // Tính toán sát thương sau khi trừ giáp
        int damageTaken = damageAmount - data.defense;
        if (damageTaken < 1) damageTaken = 1;

        currentHealth -= damageTaken;
        currentHealth = Mathf.Clamp(currentHealth, 0, data.maxHealth);
        Debug.Log("Player nhận " + damageTaken + " sát thương! Máu còn lại: " + currentHealth + "/" + data.maxHealth);
        if (playerHealthBar != null)
            playerHealthBar.UpdateBar(currentHealth, data.maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.LogError("GAME OVER - Player đã bị tiêu diệt!");
        gameObject.SetActive(false);
    }

    // === HỆ THỐNG MANA ===
    public bool UseMana(int manaCost)
    {
        if (data == null) return false; // Không có data thì không có mana

        if (currentMana >= manaCost)
        {
            currentMana -= manaCost;
            if (playerManaBar != null)
                playerManaBar.UpdateBar(currentMana, data.maxMana);
            return true;
        }
        return false;
    }

    // === CÁC HÀM XỬ LÝ BOUNDS (TỪ PLAYERMOVE2) ===

    // Hàm này sẽ được MapManager gọi
    public void SetBounds(SpriteRenderer bg)
    {
        backgroundForBounds = bg;
        if (backgroundForBounds == null) return;

        Bounds b = bg.bounds;
        RecalcHalfSize();

        minX = b.min.x + halfSize.x;
        maxX = b.max.x - halfSize.x;
        minY = b.min.y + halfSize.y;
        maxY = b.max.y - halfSize.y;

        // Đưa player về trong khung nếu đang ở ngoài
        Vector2 p = rb.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        if (rb.position != p) // Chỉ đặt lại vị trí nếu thực sự cần
        {
            rb.position = p;
        }

    }

    // Hàm tính toán nửa kích thước Player
    private void RecalcHalfSize()
    {
        if (col != null)
        {
            // Ưu tiên dùng Collider vì chính xác hơn
            Bounds cb = col.bounds;
            halfSize = new Vector2(cb.extents.x, cb.extents.y);
        }
        else
        {
            // Dự phòng nếu không có collider
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            halfSize = sr ? (Vector2)sr.bounds.extents : new Vector2(0.5f, 0.5f);
            Debug.LogWarning("PlayerBase không tìm thấy Collider2D để tính kích thước Bounds, đang dùng SpriteRenderer.");
        }
    }

    // Hàm kiểm tra và giới hạn vị trí trong FixedUpdate
    private void ClampPositionToBounds()
    {
        if (backgroundForBounds == null) return;

        var p = rb.position;
        float clampedX = Mathf.Clamp(p.x, minX, maxX);
        float clampedY = Mathf.Clamp(p.y, minY, maxY);

        if (clampedX != p.x || clampedY != p.y)
        {
            rb.position = new Vector2(clampedX, clampedY);
            if (clampedX != p.x) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (clampedY != p.y) rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }
}