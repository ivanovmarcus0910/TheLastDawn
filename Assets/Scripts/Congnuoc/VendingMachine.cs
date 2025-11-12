using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class VendingMachine : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 0.5f;          
    public GameObject vendingUI;             
    public Transform player;
    public TextMeshProUGUI message;

    [Header("Items for Sale")]
    public ItemData[] itemsForSale;             
    public Transform itemGridParent;            
    public GameObject itemCellPrefab;   
    public PlayerBase playerBase;   

    private bool playerInRange = false;
    [SerializeField]
    public RecylableInventoryManager inventoryManager;
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

        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactRange;

        if (!playerInRange && vendingUI.activeSelf)
            vendingUI.SetActive(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange) return;
        if (context.performed && vendingUI != null)
        {
            vendingUI.SetActive(!vendingUI.activeSelf);
            Debug.Log("Pressed Interact");
        }
    }

    private void SpawnItemCells()
    {
        if (itemGridParent == null || itemCellPrefab == null) return;

        foreach (var item in itemsForSale)
        {
            if (!item.canBeSold) continue;

            GameObject cell = Instantiate(itemCellPrefab, itemGridParent);

            Image icon = cell.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null) icon.sprite = item.icon;

            TextMeshProUGUI itemName = cell.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
            if (itemName != null) itemName.text = item.itemName;

            TextMeshProUGUI price = cell.transform.Find("Price")?.GetComponent<TextMeshProUGUI>();
            if (price != null) price.text = item.price.ToString() + " $";

            GameObject effectObj = cell.transform.Find("EffectText")?.gameObject;
            if (effectObj != null)
            {
                effectObj.SetActive(false);
                TextMeshProUGUI effectText = effectObj.GetComponent<TextMeshProUGUI>();
                if (effectText != null)
                    effectText.text = string.IsNullOrEmpty(item.effect) ? "" : item.effect;
            }

            Button buyBtn = cell.transform.Find("BuyButton")?.GetComponent<Button>();
            if (buyBtn != null)
                buyBtn.onClick.AddListener(() => BuyItem(item));

            if (icon != null)
            {
                Button iconBtn = icon.GetComponent<Button>();
                if (iconBtn != null && effectObj != null)
                {
                    iconBtn.onClick.AddListener(() =>
                    {
                        bool active = effectObj.activeSelf;
                        effectObj.SetActive(!active);
                    });
                }
            }
        }
    }

    private void BuyItem(ItemData item)
    {
        if (playerBase.data.sic > item.price)
        {
            playerBase.data.sic -= item.price;
            if (inventoryManager == null) return;
            if (inventoryManager.hasItem(item))
            {
                inventoryManager.increaseQuantity(item);
            }
            else
            {
                inventoryManager.AddInventoryItem(item);
            }
        }
        else
        {
            message.text= "Not enough money!";
        }
    }
}
