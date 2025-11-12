using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGolem : EnemyBase
{
    [Header("Golem Specific")]
    public float maxMana = 100f;
    public float currentMana = 0f;
    public float manaRegenRate = 5f;
    public float skillManaCost = 50f;
    public Transform skillLaunchPoint;
    public GameObject handProjectilePrefab;

    [Header("Nhiệm vụ Cản trở")]
    public SpaceshipEnergy spaceshipEnergy;
    public float teleportOffset = 4f;

    [Header("Leash Settings")]
    public float maxLeashRange = 50f;
    public bool isAggroed = false; 
    private Vector3 originalPosition; 
    private float originalGravityScale;

    private bool isHealing = false;
    private bool isChangingState = true;
    private bool hasHealedOnce = false;

    [Header("Skill: Falling Stones")]
    public GameObject warningMarkerPrefab; 
    public GameObject fallingStonePrefab;  
    public int numberOfStones = 3;         
    public float spawnRadius = 5f;        
    public float spawnHeight = 10f;        
    public float warningTime = 1.0f;
    public float delayBetweenStones = 0.4f;
    protected override void Start()
    {
        base.Start();

        originalPosition = transform.position;
        originalGravityScale = rb.gravityScale;

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
        if (player == null || !player.gameObject.activeSelf)
        {
            if (isAggroed)
            {
                ResetToOrigin();
            }

            return;
        }

        HandleManaRegen();

        if (!isAggroed)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= data.detectRange)
            {
                isAggroed = true;
                TeleportAndAttack();
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            float playerDistFromOrigin = Vector2.Distance(player.position, originalPosition);
            if (playerDistFromOrigin > maxLeashRange)
            {
                ResetToOrigin();
            }
            else
            {
                if (Time.time < nextAttackTime) return;

                if (!hasHealedOnce && currentHealth <= data.maxHealth * 0.5f)
                {
                    HandleHeal();
                }
                else if (currentMana >= skillManaCost)
                {
                    HandleSkill();
                }
                else
                {
                    TeleportAndAttack();
                }
            }
        }

        //if (healthBar != null)
        //{
        //    healthBar.transform.forward = Camera.main.transform.forward;
        //}
    }

    protected override void Patrol()
    {
        rb.linearVelocity = Vector2.zero;
    }

    void ResetToOrigin()
    {
        isAggroed = false;
        transform.position = originalPosition;
        rb.gravityScale = originalGravityScale;
        rb.linearVelocity = Vector2.zero;
    }

    public void ActivateAndTeleport()
    {
        isAggroed = true;
        TeleportAndAttack();
    }

    void TeleportAndAttack()
    {
        if (isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > teleportOffset + 1.0f)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
            Vector3 targetPosition = player.position - new Vector3(teleportOffset, 0, 0);
            transform.position = targetPosition;
        }

        isAttacking = true;
        animator.SetBool("isAttacking", true);
        rb.linearVelocity = Vector2.zero;
        FacePlayer();
        Invoke(nameof(ShootGolemHand), 0.5f);

        float attackAnimTime = 1.5f;

        nextAttackTime = Time.time + data.attackCooldown;
        Invoke(nameof(StopAttack), attackAnimTime);
    }

    protected override void HandleAttack()
    {
        isAggroed = true;
        TeleportAndAttack();
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
        if (isAttacking || currentMana < skillManaCost) return;

        currentMana -= skillManaCost;
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        FacePlayer();
        animator.SetTrigger("Skill");

        StartCoroutine(SpawnFallingStonesRoutine());

        float skillAnimTime = 2.0f;
        float totalSkillDuration = (numberOfStones * delayBetweenStones) + warningTime;
        nextAttackTime = Time.time + totalSkillDuration + data.attackCooldown;
        Invoke(nameof(StopAttack), skillAnimTime);
    }

    void ShootGolemHand()
    {
        if (handProjectilePrefab != null && skillLaunchPoint != null)
        {
            Vector3 directionToPlayer = (player.position - skillLaunchPoint.position).normalized;

            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            GameObject newHand = Instantiate(
                handProjectilePrefab,
                skillLaunchPoint.position,
                targetRotation 
            );
        }
    }

    private IEnumerator SpawnFallingStonesRoutine()
    {
        List<Vector3> markerPositions = new List<Vector3>();

        for (int i = 0; i < numberOfStones; i++)
        {
            Vector3 markerPos = player.position;
            markerPos.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(markerPos + Vector3.up * 50f, Vector2.down, Mathf.Infinity, groundLayer);
            if (hit.collider != null)
            {
                markerPos.y = hit.point.y;
            }
            else
            {
                markerPos.y = transform.position.y;
            }

            Instantiate(warningMarkerPrefab, markerPos, Quaternion.identity);
            markerPositions.Add(markerPos);

            yield return new WaitForSeconds(delayBetweenStones);
        }

        yield return new WaitForSeconds(warningTime);

        foreach (Vector3 pos in markerPositions)
        {
            Vector3 stoneSpawnPos = pos + (Vector3.up * spawnHeight);
            Instantiate(fallingStonePrefab, stoneSpawnPos, Quaternion.identity);

            yield return new WaitForSeconds(delayBetweenStones);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (isDead || isHealing || isChangingState) return;

        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        if (spaceshipEnergy != null)
        {
            spaceshipEnergy.CollectGolemCore();
        }
        rb.gravityScale = originalGravityScale;
        isAggroed = false;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        animator.SetTrigger("Die");

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        onDeath?.Invoke();
        DropLoot();

        Destroy(gameObject, 3.0f);
    }
}