using UnityEngine;

public class PetFollower : MonoBehaviour
{
    public Transform target;
    public float followDistance = 1.5f;
    public float moveSpeed = 3f;
    public float smoothTime = 0.1f;

    Vector3 vel = Vector3.zero;
    Animator anim;

    void Start() { anim = GetComponent<Animator>(); }

    void Update()
    {
        if (target == null) return;
        float d = Vector2.Distance(transform.position, target.position);

        if (d > followDistance)
        {
            Vector3 aim = target.position + Vector3.left * Mathf.Sign(target.localScale.x) * followDistance;
            transform.position = Vector3.SmoothDamp(transform.position, aim, ref vel, smoothTime, moveSpeed);

            // quay theo hướng di chuyển
            float dir = (target.position.x - transform.position.x);
            transform.localScale = new Vector3(dir >= 0 ? 1 : -1, 1, 1);

            anim?.SetBool("isWalking", true);
        }
        else
        {
            anim?.SetBool("isWalking", false);
        }
    }
}
