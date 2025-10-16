using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class VendingMachine : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 0.5f;          // Khoảng cách tương tác
    public GameObject vendingUI;                // UI Panel chứa danh sách mua/bán
    public Transform player;                    // Player transform

    [Header("Items for Sale")]
    public ItemData[] itemsForSale;             // Danh sách item bán
    public Transform itemGridParent;            // Nơi spawn item cells
    public GameObject itemCellPrefab;           // Prefab ô item

    private bool playerInRange = false;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (vendingUI != null)
            vendingUI.SetActive(false);

        SpawnItemCells();
    }

    private void Update()
    {
        if (player == null) return;

        // Kiểm tra khoảng cách player
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactRange;

        // Nếu player ra ngoài range, tắt UI
        if (!playerInRange && vendingUI.activeSelf)
            vendingUI.SetActive(false);
    }

    // Callback Input System
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;
        if (context.performed && vendingUI != null)
        {
            vendingUI.SetActive(!vendingUI.activeSelf);
            Debug.Log("Pressed Interact");
        }
    }

    // Spawn Item Cells theo Grid Layout
    private void SpawnItemCells()
    {
        if (itemGridParent == null || itemCellPrefab == null) return;

        foreach (var item in itemsForSale)
        {
            if (!item.canBeSold) continue;

            GameObject cell = Instantiate(itemCellPrefab, itemGridParent);

            // Gán icon
            Image icon = cell.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null) icon.sprite = item.icon;

            // Gán text (tên + giá)
            TextMeshProUGUI itemName = cell.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
            if (itemName != null) itemName.text = item.itemName;

            TextMeshProUGUI price = cell.transform.Find("Price")?.GetComponent<TextMeshProUGUI>();
            if (price != null) price.text = item.price.ToString() + " $";

            // Gán EffectText (ẩn mặc định)
            GameObject effectObj = cell.transform.Find("EffectText")?.gameObject;
            if (effectObj != null)
            {
                effectObj.SetActive(false);
                TextMeshProUGUI effectText = effectObj.GetComponent<TextMeshProUGUI>();
                if (effectText != null)
                    effectText.text = string.IsNullOrEmpty(item.effect) ? "" : item.effect;
            }

            // Gán nút Buy
            Button buyBtn = cell.transform.Find("BuyButton")?.GetComponent<Button>();
            if (buyBtn != null)
                buyBtn.onClick.AddListener(() => BuyItem(item));

            // Click icon để hiện / ẩn Effect
            if (icon != null)
            {
                Button iconBtn = icon.GetComponent<Button>();
                if (iconBtn != null && effectObj != null)
                {
                    iconBtn.onClick.AddListener(() =>
                    {
                        bool active = effectObj.activeSelf;
                        effectObj.SetActive(!active); // Toggle hiển thị
                    });
                }
            }
        }
    }

    private void BuyItem(ItemData item)
    {
        // Tích hợp với Inventory sau này 
        //if (playerInventory == null) return; 
        //if (playerInventory.money >= item.price) 
        //{ // playerInventory.AddItem(item); 
        // playerInventory.money -= item.price; 
        // Debug.Log("Mua " + item.itemName + " thành công!"); 
        //} 
        //else 
        //{ 
        // Debug.Log("Không đủ tiền mua " + item.itemName); 
        //}
    }
}
