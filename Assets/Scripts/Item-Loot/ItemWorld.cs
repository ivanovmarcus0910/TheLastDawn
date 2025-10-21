using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemWorld : MonoBehaviour
{
    public ItemData itemData;
    public int quantity = 1;

    // --- KHAI BÁO THÊM ---
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Lấy SpriteRenderer một lần khi khởi tạo
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Hàm này được gọi ngay sau khi vật phẩm được Instantiate (trong EnemyLootDropper.cs)
    public void SetItemData(ItemData data)
    {
        // Gán dữ liệu ItemData
        itemData = data;

        // Cập nhật SpriteRenderer để hiển thị hình ảnh đúng của vật phẩm
        if (spriteRenderer != null && data.icon != null)
        {
            spriteRenderer.sprite = data.icon;
        }
    }

    // --- SỬA HÀM VA CHẠM: Dùng STAY để đảm bảo sự kiện kích hoạt ổn định nhất ---
    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log("COLLISION: Co vat the va cham: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                // Debug.Log("✅ SUCCESS: Player da nhặt vat pham: " + itemData.itemName);

                // 1. Thêm vật phẩm vào kho đồ
                inventory.AddItem(itemData, quantity);

                // 2. Xóa vật phẩm khỏi Scene
                Destroy(gameObject);
            }
            else
            {
                // Debug.LogError("❌ ERROR: Player khong co component PlayerInventory!");
            }
        }
    }

    // Giữ nguyên hàm Reset()
    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
}