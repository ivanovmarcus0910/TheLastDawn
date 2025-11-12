using UnityEngine;
using UnityEngine.InputSystem;

public class BirdChild : MonoBehaviour
{
    [Header("Bắn")]
    public GameObject bulletPrefab;
    public Transform firePoint;
   

    [Header("Cài đặt bắn")]
    public float fireRate = 5f;
    private float nextFireTime = 0f;

    [Header("Animation")]
    public Animator animator; 

    private Transform playerTransform;

    void Start()
    {
        playerTransform = transform.parent;
    }
    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Time.time >= nextFireTime)
            {
                PerformShoot(); 
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    public void PerformShoot()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log(">>> Đã set trigger Attack!");
        }

        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bulletScript = bulletObject.GetComponent<Bullet>();
        if (bulletScript != null  )
        {
            Vector2 shootDirection = Vector2.right;
            if (playerTransform != null && playerTransform.localScale.x < 0)
                shootDirection = Vector2.left;

            bulletScript.Setup(shootDirection, 2);
        }
        else
        {
            Debug.LogError("Prefab của đạn thiếu script 'Bullet'!");
            Destroy(bulletObject);
        }
    }
}
