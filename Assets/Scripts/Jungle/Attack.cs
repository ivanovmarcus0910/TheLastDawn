using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator; // tham chiếu Animator của Enemy

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // chỉ knockback khi đang attack
        if (animator.GetBool("HasTarget"))
        {
            if (((1 << collision.gameObject.layer) & playerLayer) != 0)
            {
                Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 direction = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
