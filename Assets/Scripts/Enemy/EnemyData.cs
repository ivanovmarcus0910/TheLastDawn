using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/Create New Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string enemyName = "Enemy";
    public Sprite sprite;

    [Header("Chỉ số")]
    public float moveSpeed = 0.6f;
    public int maxHealth = 3;
    public int attackDamage = 1;
    public float detectRange = 0.2f;

    [Header("Tùy chọn")]
    public float groundCheckDistance = 0.5f;
    public float attackCooldown = 1f;
}

