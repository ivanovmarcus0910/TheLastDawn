using UnityEngine;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    InputAction moveAction;
    Vector3 position;
    [SerializeField] float speed;
    Vector3 scale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        scale = transform.localScale;
        position.x += moveValue.x * speed * Time.deltaTime;
        position.y += moveValue.y * speed * Time.deltaTime;
        transform.position = position;

        if (moveValue.x > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (moveValue.x < 0)
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }
}
