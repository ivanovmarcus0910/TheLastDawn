// File: PlayerShooting.cs
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Bắn")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Âm thanh")]
    public AudioSource sfxSource;   // Kéo AudioSource vào (cùng Player/Gun)
    public AudioClip shootSfx;      // Kéo file âm thanh bắn vào

    void Awake()
    {
        // Nếu quên gán, tự lấy AudioSource trên cùng GameObject (nếu có)
        if (sfxSource == null) TryGetComponent(out sfxSource);
    }

    // Hàm này được gọi bởi WeaponManager (đã kiểm soát fireRate bên ngoài)
    public void PerformShoot()
    {
        // 1) Tạo đạn
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2) Lấy script Bullet
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

        // 3) Thiết lập hướng bắn
        if (bulletScript != null)
        {
            Vector2 shootDirection = transform.right; // mặc định quay phải
            if (transform.localScale.x < 0) shootDirection = -transform.right;

            bulletScript.Setup(shootDirection);

            // 4) Phát âm thanh (non-blocking, không ảnh hưởng fireRate)
            if (sfxSource != null && shootSfx != null)
                sfxSource.PlayOneShot(shootSfx);
        }
        else
        {
            Debug.LogError("Prefab của đạn thiếu script 'Bullet'!");
            Destroy(bulletObject);
        }
    }
}
