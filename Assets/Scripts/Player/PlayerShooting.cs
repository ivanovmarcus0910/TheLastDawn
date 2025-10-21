// File: PlayerShooting.cs
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    // Hàm này được gọi bởi WeaponManager
    public void PerformShoot()
    {
        // 1. Tạo viên đạn VÀ LƯU LẠI một tham chiếu đến nó
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2. Lấy component script "Bullet" từ viên đạn vừa tạo
        Bullet bulletScript = bulletObject.GetComponent<Bullet>();

        // 3. Kiểm tra xem script có tồn tại không để tránh lỗi
        if (bulletScript != null)
        {
            // 4. Xác định hướng bắn (dựa vào hướng mặt của Player)
            Vector2 shootDirection = transform.right; // Mặc định là bên phải

            // Nếu Player đang quay mặt sang trái (scale.x < 0)
            if (transform.localScale.x < 0)
            {
                shootDirection = -transform.right; // Đổi hướng thành bên trái
            }

            // 5. Gọi hàm Setup() và truyền hướng bắn vào cho viên đạn
            bulletScript.Setup(shootDirection);
        }
        else
        {
            Debug.LogError("Prefab của đạn thiếu script 'Bullet'!");
        }
    }
}
