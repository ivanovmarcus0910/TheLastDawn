using UnityEngine;

[RequireComponent(typeof(AudioSource))] // đảm bảo có AudioSource
public class PlayerSwordAttack : MonoBehaviour
{
    [Header("Kiếm & Animation")]
    public Collider2D swordCollider;      // Kéo Collider2D của SwordSprite vào
    private Animator swordAnimator;

    [Header("Âm thanh chém")]
    public AudioClip slashSound;          // Kéo file âm thanh chém vào đây
    private AudioSource audioSource;      // Nội bộ để phát tiếng

    void Awake() // <-- THAY Start() BẰNG Awake()
    {
        // Lấy Animator
        swordAnimator = GetComponent<Animator>();

        // Lấy AudioSource trên cùng object
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("PlayerSwordAttack yêu cầu AudioSource nhưng không tìm thấy!");
        }
    }

    // Hàm này sẽ được WeaponManager gọi để ra lệnh chém
    public void PerformAttack()
    {
        // Gọi animation
        swordAnimator.SetTrigger("Slash");

        // Phát âm thanh chém (ngay khi ra đòn)
        PlaySlashSound();
    }

    // --- CÁC HÀM CHO ANIMATION EVENT ---

    // Bật hitbox khi chém
    public void EnableHitbox()
    {
        swordCollider.enabled = true;
    }

    // Tắt hitbox khi chém xong
    public void DisableHitbox()
    {
        swordCollider.enabled = false;
    }

    // --- Hàm riêng phát âm thanh ---
    private void PlaySlashSound()
    {
        if (audioSource != null && slashSound != null)
        {
            audioSource.PlayOneShot(slashSound);
        }
    }
}
