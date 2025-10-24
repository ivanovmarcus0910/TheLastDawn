﻿using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBase : MonoBehaviour
{
    [Header("Hồ sơ Nhân vật")]
    public PlayerData data; // Kéo file PlayerData.asset vào đây

    [Header("UI")]
    public HealthBar playerHealthBar;
    public HealthBar playerManaBar;

    [Header("Input")]
    public InputActionAsset inputActions; // Kéo file .inputactions (phải có "Player" map)

    // === CÁC BIẾN NỘI BỘ ===
    private int currentHealth;
    private int currentMana;
    private Rigidbody2D rb;
    private Animator anim;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;
    private int jumpCount = 0;

    // --- BOUNDS ---
    private Collider2D col;
    private SpriteRenderer backgroundForBounds;
    private float minX, maxX, minY, maxY;
    private Vector2 halfSize;

    // === KHỞI TẠO ===
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        RecalcHalfSize();
    }

    void Start()
    {
        // --- DATA ---
        if (data == null)
        {
            Debug.LogError("⚠️ Chưa gán PlayerData cho PlayerBase!");
            enabled = false;
            return;
        }

        currentHealth = data.maxHealth;
        currentMana = data.maxMana;

        playerHealthBar?.UpdateBar(currentHealth, data.maxHealth);
        playerManaBar?.UpdateBar(currentMana, data.maxMana);

        // --- INPUT SYSTEM ---
        try
        {
            if (inputActions == null)
                throw new System.Exception("Chưa gán InputActionAsset!");

            var map = inputActions.FindActionMap("Player", true);
            map.Enable();
            moveAction = map.FindAction("Move", true);
            jumpAction = map.FindAction("Jump", true);

            jumpAction.performed += _ => Jump();
            Debug.Log($"✅ Action Map Enabled: {map.enabled}");
            moveAction.Enable();
            jumpAction.Enable();

            Debug.Log("✅ Input Actions Loaded: Move & Jump");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Input setup error: {e.Message}");
            enabled = false;
        }
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
    //void OnDisable()
    //{
    //    moveAction?.Disable();
    //    jumpAction?.Disable();
    //}

    // === XỬ LÝ INPUT ===
    void Update()
    {
        // --- Đọc Input ---
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
            Debug.Log($"Move Input: {moveInput}");
        }
        //else moveInput = Vector2.zero;

        // --- Flip nhân vật ---
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Sign(moveInput.x) * Mathf.Abs(s.x);
            transform.localScale = s;
        }

        // --- Animation ---
        bool isRunning = Mathf.Abs(moveInput.x) > 0.01f;
        anim?.SetBool("run", isRunning);

        // --- Debug bounds ---
        Debug.DrawLine(new Vector3(minX, minY), new Vector3(minX, maxY), Color.red);
        Debug.DrawLine(new Vector3(maxX, minY), new Vector3(maxX, maxY), Color.red);
        Debug.DrawLine(new Vector3(minX, minY), new Vector3(maxX, minY), Color.red);
        Debug.DrawLine(new Vector3(minX, maxY), new Vector3(maxX, maxY), Color.red);
    }

    // === DI CHUYỂN ===
    void FixedUpdate()
    {
        float currentMoveSpeed = (data != null) ? data.moveSpeed : 0f;
        rb.linearVelocity = new Vector2(moveInput.x * currentMoveSpeed, rb.linearVelocity.y);
        ClampPositionToBounds();
    }

    // === NHẢY ===
    void Jump()
    {
        if (data == null || jumpCount >= 2) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * data.jumpForce, ForceMode2D.Impulse);
        jumpCount++;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.5f)
            {
                jumpCount = 0;
                break;
            }
        }
    }

    // === MÁU ===
    public void TakeDamage(int damageAmount)
    {
        if (data == null) return;

        int damageTaken = Mathf.Max(1, damageAmount - data.defense);
        currentHealth = Mathf.Clamp(currentHealth - damageTaken, 0, data.maxHealth);

        Debug.Log($"💥 Player nhận {damageTaken} sát thương! Máu còn lại: {currentHealth}/{data.maxHealth}");
        playerHealthBar?.UpdateBar(currentHealth, data.maxHealth);

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.LogError("💀 GAME OVER - Player đã bị tiêu diệt!");
        gameObject.SetActive(false);
    }

    // === MANA ===
    public bool UseMana(int manaCost)
    {
        if (data == null || currentMana < manaCost) return false;
        currentMana -= manaCost;
        playerManaBar?.UpdateBar(currentMana, data.maxMana);
        return true;
    }

    // === BOUNDS ===
    public void SetBounds(SpriteRenderer bg)
    {
        backgroundForBounds = bg;
        if (bg == null) return;

        Bounds b = bg.bounds;
        RecalcHalfSize();

        minX = b.min.x + halfSize.x;
        maxX = b.max.x - halfSize.x;
        minY = b.min.y + halfSize.y;
        maxY = b.max.y - halfSize.y;

        Vector2 p = rb.position;
        p.x = Mathf.Clamp(p.x, minX, maxX);
        p.y = Mathf.Clamp(p.y, minY, maxY);
        rb.position = p;

        Debug.Log($"✅ SetBounds: {bg.name} [{minX}, {maxX}] [{minY}, {maxY}]");
    }

    private void RecalcHalfSize()
    {
        if (col != null)
        {
            Bounds cb = col.bounds;
            halfSize = cb.extents;
        }
        else
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            halfSize = sr ? (Vector2)sr.bounds.extents : new Vector2(0.5f, 0.5f);
        }
    }

    private void ClampPositionToBounds()
    {
        if (backgroundForBounds == null) return;

        Vector2 p = rb.position;
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
