using UnityEngine;

public class HandProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10; 
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerBase>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
