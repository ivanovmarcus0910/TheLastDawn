using UnityEngine;

public class dieLine : MonoBehaviour
{
    public GameObject player;
    private PlayerBase scriptPlayerbase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        scriptPlayerbase = player.GetComponent<PlayerBase>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("chạm Player");
            scriptPlayerbase.Die();
        }

    }
}
