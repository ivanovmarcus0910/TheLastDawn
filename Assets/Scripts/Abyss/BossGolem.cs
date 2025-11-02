using UnityEngine;
using System.Collections;
using System;

public class BossGolem : EnemyBase
{
    [Header("Golem Specific")]
    public float maxMana = 100f;
    public float currentMana = 0f;
    public float manaRegenRate = 5f;
    public float skillManaCost = 50f;
    public Transform skillLaunchPoint;

    private bool isHealing = false;
    private bool isChangingState = true;
    private bool hasHealedOnce = false;

    protected override void Start()
    {
        base.Start();

        isAttacking = false;
        rb.linearVelocity = Vector2.zero;

        if (isChangingState)
        {
            animator.SetTrigger("ChangeState");
            Invoke(nameof(EndChangeState), 2f);
        }

        currentMana = 0f;
    }

    protected override void Update()
    {
        if (isDead || isHurt || isHealing || isChangingState) return;

        float distance = Vector2.Distance(transform.position, player.position);

        HandleManaRegen();

        if (currentMana >= skillManaCost)
        {
            HandleSkill();
        }
        else if (!hasHealedOnce && currentHealth <= data.maxHealth * 0.5f)
        {
            HandleHeal();
        }
        else if (distance <= data.detectRange)
        {
            HandleAttack();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (healthBar != null)
        {
            healthBar.transform.forward = Camera.main.transform.forward;
        }
    }

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
        currentMana = Mathf.Min(currentMana + manaRegenRate * Time.deltaTime, maxMana);
    }

    void HandleHeal()
    {
        isHealing = true;
        hasHealedOnce = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Heal");

        StartCoroutine(PerformHeal());
    }

    private IEnumerator PerformHeal()
    {
        yield return new WaitForSeconds(3f);

        int healAmount = Mathf.CeilToInt(data.maxHealth * 0.2f);
        currentHealth = Mathf.Min(currentHealth + healAmount, data.maxHealth);

        if (healthBar != null)
            StartCoroutine(UpdateHealthBarSmooth(currentHealth, data.maxHealth));

        isHealing = false;
    }

    void HandleSkill()
    {
        if (isAttacking) return;

        currentMana -= skillManaCost;
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        FacePlayer();
        animator.SetTrigger("Skill");

        Invoke(nameof(ShootGolemHand), 0.5f);

        Invoke(nameof(StopAttack), 1.5f);
    }

    public GameObject handProjectilePrefab;

    void ShootGolemHand()
    {
        if (handProjectilePrefab != null && skillLaunchPoint != null)
        {
            GameObject newHand = Instantiate(
                handProjectilePrefab,
                skillLaunchPoint.position,
                skillLaunchPoint.rotation
            );

            // Tùy chỉnh tốc độ/hướng bay (ví dụ nếu tay cần bay theo hướng của boss)
            // newHand.GetComponent<HandProjectile>().SetDirection(currentDirection);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isDead || isHealing || isChangingState) return;

        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
    }
}