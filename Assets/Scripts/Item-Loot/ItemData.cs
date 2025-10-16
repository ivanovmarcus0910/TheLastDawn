using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Create New Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int price;
    public Sprite icon;
    public GameObject itemPrefab;   // Prefab vật phẩm thật trong scene
    [TextArea] public string effect; // Mô tả effect, ví dụ "Hồi 10 HP"
    public bool canBeSold = true;    // Có thể bán trong cửa hàng
}
