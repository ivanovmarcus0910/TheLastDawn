using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Chỉ số Chiến đấu")]
    public int maxHealth ;
    public int maxMana ;
    public float moveSpeed;
    public int defense;
    public int baseDamage; // <--- THÊM DÒNG NÀY

    [Header("Chỉ số Nhảy")]
    public float jumpForce;
}