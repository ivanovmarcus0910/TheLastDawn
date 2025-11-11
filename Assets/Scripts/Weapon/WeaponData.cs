using UnityEngine;

// Bỏ enum EquipmentSlot ra khỏi đây
public enum WeaponType { Sword, Gun }

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Thông tin Cơ bản")]
    public string weaponName;
    public Sprite icon;
    public WeaponType weaponType; // Đây là Súng hay Kiếm?

    [Header("Prefab & Chỉ số")]
    // Prefab của vũ khí (Gun hoặc SwordPivot)
    public GameObject weaponPrefab;

    [Header("Chỉ số Chiến đấu")]
    public int weaponDamage; // Sát thương CƠ BẢN của vũ khí này
    public float fireRate; // Tốc độ bắn (0 nếu là kiếm)

    [Header("Chỉ số Súng (Bỏ trống nếu là kiếm)")]
    // THÊM MỚI: Loại đạn riêng cho súng này
    public GameObject bulletPrefab;
}