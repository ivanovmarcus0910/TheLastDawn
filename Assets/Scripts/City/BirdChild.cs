using UnityEngine;
using UnityEngine.InputSystem;

public class BirdChild : MonoBehaviour
{
    [Header("Bắn")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Cài đặt bắn")]
    public float fireRate = 5f; // Số viên đạn bắn ra mỗi giây
    private float nextFireTime = 0f; // Thời điểm được phép bắn viên tiếp theo

    [Header("Animation")]
    public Animator animator; // Kéo Animator của chim vào đây

  

    void Update()
    {
        // Kiểm tra chuột trái (Input System)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Time.time >= nextFireTime)
            {
                PerformShoot(); // Bắn
                nextFireTime = Time.time + 1f / fireRate; // Cập nhật cooldown
            }
        }
    }

    public void PerformShoot()
    {
        // Gọi animation Attack
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log(">>> Đã set trigger Attack!");
        }

        // 1) Tạo đạn
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2) Lấy script Bullet
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

        // 3) Thiết lập hướng bắn
        if (bulletScript != null)
        {
            Vector2 shootDirection = transform.right;
            if (transform.localScale.x < 0) shootDirection = -transform.right;

            bulletScript.Setup(shootDirection);
        }
        else
        {
            Debug.LogError("Prefab của đạn thiếu script 'Bullet'!");
            Destroy(bulletObject);
        }
    }
}
