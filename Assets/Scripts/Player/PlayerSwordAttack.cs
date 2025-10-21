using UnityEngine;

public class PlayerSwordAttack : MonoBehaviour
{
    // Kéo Collider2D của SwordSprite vào đây
    public Collider2D swordCollider;
    private Animator swordAnimator;

    void Start()
    {
        // Tự lấy Animator đang gắn trên cùng đối tượng (SwordPivot)
        swordAnimator = GetComponent<Animator>();
    }

    // Hàm này sẽ được WeaponManager gọi để ra lệnh chém
    public void PerformAttack()
    {
        swordAnimator.SetTrigger("Slash");
    }

    // --- CÁC HÀM CHO ANIMATION EVENT ---

    // Hàm để BẬT hitbox khi chém
    public void EnableHitbox()
    {
        swordCollider.enabled = true;
    }

    // Hàm để TẮT hitbox khi chém xong
    public void DisableHitbox()
    {
        swordCollider.enabled = false;
    }
}