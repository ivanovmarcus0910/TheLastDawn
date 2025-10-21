using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReactorUIManager : MonoBehaviour
{
    public static ReactorUIManager Instance;

    [Header("UI References")]
    public GameObject panel;
    public Transform ingredientsContainer;
    public GameObject ingredientSlotPrefab;
    public Button combineButton;
    public Image progressFill;
    public Transform resultSlot; // chính là "Result" node
    public TextMeshProUGUI hintText;
    [SerializeField]
    public RecylableInventoryManager inventoryManager;

    [Header("Reactor Settings")]
    public CraftRecipe currentRecipe;

    private List<GameObject> currentSlots = new List<GameObject>();
    private bool isCrafting = false;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        progressFill.fillAmount = 0;
    }

    public void ShowUI(CraftRecipe recipe)
    {
        currentRecipe = recipe;
        panel.SetActive(true);
        progressFill.fillAmount = 0;

        // Xoá slot cũ
        foreach (var slot in currentSlots)
            Destroy(slot);
        currentSlots.Clear();

        // Hiển thị các nguyên liệu
        foreach (var ing in recipe.ingredients)
        {
            GameObject slot = Instantiate(ingredientSlotPrefab, ingredientsContainer);
            currentSlots.Add(slot);

            slot.transform.Find("Icon").GetComponent<Image>().sprite = ing.item.icon;
            slot.transform.Find("AmountText").GetComponent<TextMeshProUGUI>().text = "x" + ing.amount;
        }

        // Kết quả
        var resultIcon = resultSlot.Find("IconResult").GetComponent<Image>();
        var resultText = resultSlot.Find("ResultText").GetComponent<TextMeshProUGUI>();

        resultIcon.sprite = recipe.result.icon;
        resultText.text = $"→ {recipe.result.itemName}";

        // Gán sự kiện nút Combine
        //combineButton.onClick.RemoveAllListeners();
        combineButton.onClick.AddListener(OnClickCombine);
    }

    public void HideUI()
    {
        panel.SetActive(false);
    }

    public void OnClickCombine()
    {
        print("Entered");

        if (isCrafting) return;
        print("Entered");

        StartCoroutine(CraftRoutine());
    }

    private IEnumerator CraftRoutine()
    {
        // Kiểm tra đủ nguyên liệu
        bool canCraft = true;
        foreach (var ing in currentRecipe.ingredients)
        {
            if (inventoryManager.getQuantity(ing.item)<ing.amount)
            {
                canCraft = false;
                break;
            }
        }
        var resultText = resultSlot.Find("ResultText").GetComponent<TextMeshProUGUI>();

        if (!canCraft)
        {

            resultText.text = "⚠️ Not enough materials!";
            yield break;
        }

        // Hiệu ứng chế tạo
        isCrafting = true;
        resultText.text = "Processing...";
        float t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            progressFill.fillAmount = t / 2f;
            yield return null;
        }

        // Trừ nguyên liệu
        foreach (var ing in currentRecipe.ingredients)
            inventoryManager.decreaseQuantity(ing.item, ing.amount);

        //Thêm kết quả
        inventoryManager.AddInventoryItem(currentRecipe.result);

        progressFill.fillAmount = 1f;
        resultText.text = $"✅ Created: {currentRecipe.result.itemName}";
        isCrafting = false;
    }
}
