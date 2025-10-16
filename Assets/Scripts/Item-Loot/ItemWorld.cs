using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemWorld : MonoBehaviour
{
    public ItemData itemData;
    public int quantity = 1;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("COLLISION: Co vat the va cham: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                //Debug.Log("✅ SUCCESS: Player da nhặt vat pham: " + itemData.itemName);
                inventory.AddItem(itemData, quantity);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("❌ ERROR: Player khong co component PlayerInventory!");
            }
        }
    }
}
