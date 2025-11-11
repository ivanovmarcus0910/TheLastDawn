using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemWorld : MonoBehaviour
{
    public ItemData itemData;
    public int quantity = 1;

    private SpriteRenderer spriteRenderer;
    private bool isBeingPickedUp = false; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetItemData(ItemData data)
    {
        itemData = data;

        if (spriteRenderer != null && data.icon != null)
        {
            spriteRenderer.sprite = data.icon;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isBeingPickedUp) return; 
            isBeingPickedUp = true;     
            

            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                // === BÁO CÁO QUEST ===
                if (QuestManager.Instance != null && itemData != null)
                {
                    QuestManager.Instance.UpdateQuestProgress(ObjectiveType.Collect, itemData.itemName, quantity);
                }
               

                Debug.Log($"[DEBUG] Nhat Item: {itemData.itemName}, So luong ItemWorld: {quantity}");

                inventory.AddItem(itemData, quantity);

                
                Destroy(gameObject);
            }
        }
    }

    void Reset()
    {
       
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
}