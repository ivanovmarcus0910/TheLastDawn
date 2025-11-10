using UnityEngine;

public class FallingStone : MonoBehaviour
{
    public int damage = 15;
    public GameObject impactEffectPrefab;
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBase playerBase = other.GetComponent<PlayerBase>();
            if (playerBase != null)
            {
                playerBase.TakeDamage(damage);
            }

            SpawnImpactEffect();
            Destroy(gameObject);
        }

        else if (other.CompareTag("Ground"))
        {
            SpawnImpactEffect();
            Destroy(gameObject);
        }

        void SpawnImpactEffect()
        {
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}