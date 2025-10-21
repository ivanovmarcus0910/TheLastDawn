using UnityEngine;
using System.Collections;
using System;

// Script BossGolem kế thừa từ EnemyBase
public class BossGolem : EnemyBase
{
    [Header("Golem Specific")]
    public float maxMana = 100f;
    public float currentMana = 0f;
    public float manaRegenRate = 5f; // Mana hồi mỗi giây
    public float skillManaCost = 50f;
    public Transform skillLaunchPoint; // Vị trí tay Golem sẽ tách ra

    private bool isHealing = false;
    private bool isChangingState = true; // Bắt đầu ở trạng thái đổi trạng thái
    private bool hasHealedOnce = false; // Đánh dấu đã hồi máu lần đầu chưa

    protected override void Start()
    {
        // Gọi hàm Start của lớp cha để khởi tạo animator, health, player, v.v.
        base.Start();

        // Đặt trạng thái ban đầu: Golem không di chuyển, đang đổi trạng thái
        isAttacking = false;
        rb.linearVelocity = Vector2.zero;

        if (isChangingState)
        {
            animator.SetTrigger("ChangeState");
            // Gọi hàm kết thúc changeState sau thời gian animation (ví dụ 2 giây)
            Invoke(nameof(EndChangeState), 2f);
        }

        currentMana = 0f;
    }

    protected override void Update()
    {
        if (isDead || isHurt || isHealing || isChangingState) return;

        // Thực hiện hành vi của Golem
        float distance = Vector2.Distance(transform.position, player.position);

        HandleManaRegen();

        // 1. Ưu tiên Skill (nếu đủ mana)
        if (currentMana >= skillManaCost)
        {
            HandleSkill();
        }
        // 2. Ưu tiên Heal (nếu dưới 50% máu và chưa hồi lần nào)
        else if (!hasHealedOnce && currentHealth <= data.maxHealth * 0.5f)
        {
            HandleHeal();
        }
        // 3. Ưu tiên Attack (nếu Player trong tầm)
        else if (distance <= data.detectRange)
        {
            HandleAttack();
        }
        // 4. Nếu không có gì xảy ra, đứng yên (Không gọi base.Patrol)
        else
        {
            // Đảm bảo boss dừng hẳn khi không làm gì
            rb.linearVelocity = Vector2.zero;
        }

        // Cập nhật hướng của thanh máu (giống base)
        if (healthBar != null)
        {
            healthBar.transform.forward = Camera.main.transform.forward;
        }
    }

    // Golem không tuần tra, nên hàm này sẽ trống (override để vô hiệu hóa)
    protected override void Patrol()
    {
        rb.linearVelocity = Vector2.zero;
    }

    void EndChangeState()
    {
        isChangingState = false;
    }

    void HandleManaRegen()
    {
        // Hồi mana liên tục (trừ khi đang chết/hồi máu)
        currentMana = Mathf.Min(currentMana + manaRegenRate * Time.deltaTime, maxMana);
    }

    void HandleHeal()
    {
        isHealing = true;
        hasHealedOnce = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Heal");

        // Bắt đầu Coroutine hồi máu
        StartCoroutine(PerformHeal());
    }

    private IEnumerator PerformHeal()
    {
        // Chờ animation heal kết thúc (ví dụ 3 giây)
        yield return new WaitForSeconds(3f);

        // Hồi máu: tăng 20% máu tối đa
        int healAmount = Mathf.CeilToInt(data.maxHealth * 0.2f);
        currentHealth = Mathf.Min(currentHealth + healAmount, data.maxHealth);

        if (healthBar != null)
            StartCoroutine(UpdateHealthBarSmooth(currentHealth, data.maxHealth));

        // Kết thúc hồi máu
        isHealing = false;
    }

    void HandleSkill()
    {
        if (isAttacking) return; // Tránh dùng skill khi đang tấn công thường

        currentMana -= skillManaCost;
        isAttacking = true; // Coi skill là một dạng tấn công
        rb.linearVelocity = Vector2.zero;
        FacePlayer();
        animator.SetTrigger("Skill");

        // Dùng Invoke để gọi hàm tạo tay đạn (hoặc dùng Animation Event)
        // Đây là bước quan trọng để bắn tay golem ra
        Invoke(nameof(ShootGolemHand), 0.5f);

        // Kết thúc skill sau một thời gian
        Invoke(nameof(StopAttack), 1.5f);
    }

    // ============================
    // BẮN TAY GOLEM (KẾT HỢP VỚI BÀI TRƯỚC)
    // ============================

    public GameObject handProjectilePrefab; // Cần kéo Prefab tay vào Inspector

    // Hàm này được gọi bởi Invoke từ HandleSkill
    void ShootGolemHand()
    {
        // Kiểm tra xem đã có prefab và điểm bắn chưa
        if (handProjectilePrefab != null && skillLaunchPoint != null)
        {
            // Tạo tay đạn tại vị trí skillLaunchPoint và hướng xoay của nó
            GameObject newHand = Instantiate(
                handProjectilePrefab,
                skillLaunchPoint.position,
                skillLaunchPoint.rotation
            );

            // Tùy chỉnh tốc độ/hướng bay (ví dụ nếu tay cần bay theo hướng của boss)
            // Ví dụ: newHand.GetComponent<HandProjectile>().SetDirection(currentDirection);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isDead || isHealing || isChangingState) return; // Không bị sát thương khi đang heal/changeState

        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
        // (Thêm hiệu ứng chết boss đặc biệt ở đây nếu cần)
    }
}