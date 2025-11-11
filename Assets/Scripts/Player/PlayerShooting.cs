using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerShooting : MonoBehaviour
{
    [Header("Thiết lập Bắn (Chung)")]
    // BỎ: public GameObject bulletPrefab;
    public Transform firePoint;
    public AudioClip shootSfx; // Giữ lại file âm thanh chung

    // Các biến nội bộ, sẽ được WeaponManager nạp vào
    private WeaponData myWeaponData; // Sẽ lưu trữ data của súng này
    private PlayerBase playerBase;
    private AudioSource sfxSource;

    void Awake()
    {
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null) Debug.LogError("PlayerShooting thiếu AudioSource!");
    }

    // Hàm MỚI: WeaponManager sẽ gọi hàm này khi trang bị súng
    public void Initialize(WeaponData weaponData, PlayerBase player)
    {
        this.myWeaponData = weaponData;
        this.playerBase = player;
    }

    // Hàm PerformShoot giờ sẽ đọc thông tin từ myWeaponData
    public void PerformShoot()
    {
        Debug.Log("PlayerShooting: Đã nhận lệnh, chuẩn bị bắn!");
        // Kiểm tra xem đã được nạp data và bullet prefab chưa
        if (myWeaponData == null || myWeaponData.bulletPrefab == null)
        {
            Debug.LogError("Súng chưa được nạp WeaponData hoặc thiếu Bullet Prefab!");
            return;
        }

        // 1. Lấy bullet prefab TỪ DATA
        GameObject bulletObject = Instantiate(myWeaponData.bulletPrefab, firePoint.position, firePoint.rotation);

        // 2. Lấy script Bullet
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

        if (bulletScript != null && playerBase != null && playerBase.data != null)
        {
            // 3. Xác định hướng bắn (dựa vào PlayerBase)
            Vector2 shootDirection = transform.right;
            if (playerBase.transform.localScale.x < 0) shootDirection = -transform.right;

            // 4. Tính tổng sát thương TỪ DATA
            int totalDamage = myWeaponData.weaponDamage + playerBase.data.baseDamage;

            // 5. Truyền hướng VÀ sát thương cho đạn
            bulletScript.Setup(shootDirection, totalDamage);

            // 6. Phát âm thanh
            if (sfxSource != null && shootSfx != null)
                sfxSource.PlayOneShot(shootSfx);
        }
        else
        {
            Debug.LogError("Lỗi khi bắn, thiếu script Bullet, PlayerBase hoặc PlayerData!");
            Destroy(bulletObject); // Hủy đạn nếu có lỗi
        }
    }
}