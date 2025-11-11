using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemWorld : MonoBehaviour
{
    public ItemData itemData;
    public int quantity = 1;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void SetItemData(ItemData data)
    {
        itemData = data; 
        
        // Cập nhật SpriteRenderer để hiển thị hình ảnh đúng của vật phẩm
        if (spriteRenderer != null && data.icon != null)
        {
            spriteRenderer.sprite = data.icon; 
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                // === THÊM ĐOẠN NÀY ĐỂ BÁO CÁO QUEST ===
                if (QuestManager.Instance != null && itemData != null)
                {
                    // Báo cho QuestManager biết 1 item đã được nhặt
                    // (Chúng ta dùng itemData.itemName hoặc itemData.name)
                    QuestManager.Instance.UpdateQuestProgress(ObjectiveType.Collect, itemData.itemName, quantity);
                }
                // =====================================

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