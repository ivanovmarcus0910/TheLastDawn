using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float delay = 2f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}
