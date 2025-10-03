using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackCooldown = 0.5f;

    private Animator anim;
    private float nextAttackTime = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack()
    {
        // 1. Kích hoạt animation
        anim.SetTrigger("attack");

        // 2. Xác định hướng bắn dựa vào hướng nhân vật đang quay mặt
        Vector2 direction = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;

        // 3. Tạo viên đạn và truyền hướng cho nó
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Setup(direction);
    }
}

