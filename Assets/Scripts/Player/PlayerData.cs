using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Chỉ số Chiến đấu")]
    public int maxHealth = 100;
    public int maxMana = 50;
    public float moveSpeed = 5f;
    public int defense = 0;
    public int baseDamage = 5; // <--- THÊM DÒNG NÀY

    [Header("Chỉ số Nhảy")]
    public float jumpForce = 10f;
}