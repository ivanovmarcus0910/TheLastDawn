using UnityEngine;
using System;

public class EnemyBase : MonoBehaviour
{
    [Header("Dữ liệu quái")]
    public EnemyData data; // Gán asset ScriptableObject ở đây
    public LootTable lootTable;
    public Transform player;
    public LayerMask groundLayer;
    // <--- THÊM DÒNG NÀY: Sự kiện báo hiệu quái vật đã chết
    public event Action onDeath;

    protected Animator animator;
    protected Rigidbody2D rb;

    protected int currentHealth;
    protected bool isAttacking = false;
    protected bool isDead = false;
    protected bool isHurt = false;
    protected int currentDirection = -1;
    protected float halfWidth;
    private float nextAttackTime = 0f;

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

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        isHurt = true;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke(nameof(RecoverFromHurt), 0.3f);
        }
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
        onDeath?.Invoke();
        DropLoot();

        Destroy(gameObject, 1.5f);
    }
    void DropLoot()
    {
        if (lootTable == null || lootTable.lootItems.Length == 0) return;

        foreach (var loot in lootTable.lootItems)
        {
            float roll = UnityEngine.Random.value; // 0 → 1
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
