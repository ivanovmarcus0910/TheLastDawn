using UnityEngine;
using System.Collections; // cần cho IEnumerator
using TMPro;
using UnityEngine.UI;
using System;

public class EnemyBase : MonoBehaviour
{
    [Header("Dữ liệu quái")]
    public EnemyData data;
    public LootTable lootTable;
    public Transform player;
    public LayerMask groundLayer;

    [Header("Health Bar")]
    public HealthBar healthBar;

    protected Animator animator;
    protected Rigidbody2D rb;

    protected int currentHealth;
    protected bool isAttacking = false;
    protected bool isDead = false;
    protected bool isHurt = false;
    protected int currentDirection = -1;
    protected float halfWidth;
    private float nextAttackTime = 0f;

    public Action onDeath;
    private float lastTimeDamaged;      // Thời điểm bị đánh gần nhất
    private Coroutine regenCoroutine;   // Dùng để dừng hồi máu nếu bị đánh lại

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
            halfWidth = box.size.x / 2f * transform.localScale.x;

        currentHealth = data.maxHealth;
        lastTimeDamaged = Time.time;

        // Khởi tạo health bar ngay lúc đầu
        if (healthBar != null)
        {
            healthBar.UpdateBar(currentHealth, data.maxHealth);
        }
    }

    protected virtual void Update()
    {
        if (isDead || isHurt) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= data.detectRange)
        {
            HandleAttack();
        }
        else
        {
            Patrol();
        }

        // Cập nhật hướng của thanh máu (luôn đối diện camera)
        if (healthBar != null)
        {
            healthBar.transform.forward = Camera.main.transform.forward;
        }
    }

    protected void HandleAttack()
    {
        if (Time.time < nextAttackTime) return;

        isAttacking = true;
        animator.SetBool("isAttacking", true);
        rb.linearVelocity = Vector2.zero;
        FacePlayer();

        // Gây sát thương cho player (nếu có script PlayerHealth)
        // player.GetComponent<PlayerHealth>()?.TakeDamage(data.attackDamage);

        nextAttackTime = Time.time + data.attackCooldown;
        Invoke(nameof(StopAttack), 0.4f); // Animation kết thúc
    }

    void StopAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    protected virtual void Patrol()
    {
        rb.linearVelocity = new Vector2(currentDirection * data.moveSpeed, rb.linearVelocity.y);

        Vector2 downCheckPos = (Vector2)transform.position + new Vector2(currentDirection * halfWidth, 0f);
        if (!Physics2D.Raycast(downCheckPos, Vector2.down, data.groundCheckDistance, groundLayer))
        {
            Flip();
        }
    }

    protected void Flip()
    {
        currentDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            currentDirection = 1;
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            currentDirection = -1;
        }
    }

    // ============================
    //  TAKE DAMAGE + HEALTH BAR
    // ============================
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        animator.SetTrigger("Hurt");
        isHurt = true;

        if (healthBar != null)
            StartCoroutine(UpdateHealthBarSmooth(currentHealth, data.maxHealth));

        lastTimeDamaged = Time.time;

        // Nếu đang hồi máu thì dừng
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke(nameof(RecoverFromHurt), 0.3f);
            regenCoroutine = StartCoroutine(RegenHealthAfterDelay());
        }
    }

    private IEnumerator UpdateHealthBarSmooth(int targetValue, int maxValue)
    {
        float startFill = healthBar.fillBar.fillAmount;
        float targetFill = (float)targetValue / maxValue;
        float t = 0f;
        float duration = 0.2f; // tốc độ mượt

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerpValue = Mathf.Lerp(startFill, targetFill, t / duration);
            healthBar.fillBar.fillAmount = lerpValue;
            healthBar.valueText.text = $"{Mathf.RoundToInt(lerpValue * maxValue)} / {maxValue}";
            yield return null;
        }

        // đảm bảo cập nhật chính xác cuối cùng
        healthBar.UpdateBar(targetValue, maxValue);
    }

    void RecoverFromHurt()
    {
        isHurt = false;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Die");

        // Ẩn thanh máu khi chết
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        onDeath?.Invoke(); // Kích hoạt sự kiện chết quái
        DropLoot();

        Destroy(gameObject, 1.5f);
    }

    // ============================
    //  HỒI MÁU SAU 5 GIÂY KHÔNG BỊ ĐÁNH
    // ============================
    private IEnumerator RegenHealthAfterDelay()
    {
        yield return new WaitForSeconds(5f);

        while (currentHealth < data.maxHealth)
        {
            // Nếu bị đánh lại thì dừng hồi
            if (Time.time - lastTimeDamaged < 5f)
                yield break;

            int regenAmount = Mathf.CeilToInt(data.maxHealth * 0.1f);
            currentHealth = Mathf.Min(currentHealth + regenAmount, data.maxHealth);

            if (healthBar != null)
                StartCoroutine(UpdateHealthBarSmooth(currentHealth, data.maxHealth));

            yield return new WaitForSeconds(1f);
        }

        regenCoroutine = null;
    }

    // ============================
    //  DROP LOOT
    // ============================
    void DropLoot()
    {
        if (lootTable == null || lootTable.lootItems.Length == 0) return;

        foreach (var loot in lootTable.lootItems)
        {
            float roll = UnityEngine.Random.value;
            if (roll <= loot.dropChance)
            {
                Instantiate(
                    loot.item.itemPrefab,
                    transform.position + Vector3.up * 0.5f,
                    Quaternion.identity
                );
            }
        }
    }
}
