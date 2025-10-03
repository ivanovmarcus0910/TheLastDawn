using UnityEngine;
using UnityEngine.InputSystem;

public class Playermove2 : MonoBehaviour
{
    InputAction moveAction;
    InputAction jumpAction;
    Vector3 position;
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpForce = 2.5f;

    Vector3 scale;
    Rigidbody2D rb;
    Animator anim;   // 👈 Animator

    int jumpCount = 0;
    [SerializeField] int maxJumps = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();  // 👈 lấy Animator

        position = transform.position;
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        jumpAction.performed += ctx => Jump();
    }

    void Update()
    {
        position = transform.position;
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        scale = transform.localScale;

        // di chuyển ngang
        position.x += moveValue.x * speed * Time.deltaTime;
        transform.position = position;

        // lật nhân vật
        if (moveValue.x > 0) scale.x = Mathf.Abs(scale.x);
        else if (moveValue.x < 0) scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;

        // 👇 điều khiển animation
        bool isRunning = Mathf.Abs(moveValue.x) > 0.01f;
        anim.SetBool("run", isRunning);
    }

    void Jump()
    {
        if (jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                jumpCount = 0;
                break;
            }
        }
    }
}
