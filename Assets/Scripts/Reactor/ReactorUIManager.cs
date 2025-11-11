using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReactorUIManager : MonoBehaviour
{
    public static ReactorUIManager Instance;

    [Header("UI References")]
    public GameObject panel; // Panel_ReactorUI
    public GameObject recipesPanel; //  Panel chứa danh sách công thức
    public Transform ingredientsContainer;
    public GameObject ingredientSlotPrefab;
    public Button combineButton;
    public Button backButton;
    public Image progressFill;
    public Transform resultSlot;
    public TextMeshProUGUI hintText;
    [SerializeField]
    public RecylableInventoryManager inventoryManager;

    [Header("Recipe List Settings")]
    public Transform recipesContainer; //  Content của ScrollView
    public GameObject recipeItemPrefab; 
    public List<CraftRecipe> allRecipes; // Danh sách công thức có thể chế tạo

    [Header("Reactor Settings")]
    public CraftRecipe currentRecipe;

    private List<GameObject> currentSlots = new List<GameObject>();
    private bool isCrafting = false;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
       recipesPanel.SetActive(false);
        progressFill.fillAmount = 0;
    }

    public void ShowRecipeList()
    {
        //MessageNPC.Instance.Show("Bạn đã chế tạo thành công Iron Sword!");
        panel.SetActive(true);
        recipesPanel.SetActive(true);
        Debug.Log("set active");

        // Xóa item cũ
        foreach (Transform child in recipesContainer)
            Destroy(child.gameObject);

        // Hiển thị danh sách recipes
        foreach (var r in allRecipes)
        {
            GameObject item = Instantiate(recipeItemPrefab, recipesContainer);
            RecipeItemUI itemUI = item.GetComponent<RecipeItemUI>();
            itemUI.Setup(r);
        }
    }

    public void ShowUI(CraftRecipe recipe)
    {
        currentRecipe = recipe;
        recipesPanel.SetActive(false);
        panel.SetActive(true);
        progressFill.fillAmount = 0;

        // Xóa slot cũ
        foreach (var slot in currentSlots)
            Destroy(slot);
        currentSlots.Clear();

        // Hiển thị nguyên liệu
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

        // Nút combine
        combineButton.onClick.RemoveAllListeners();
        combineButton.onClick.AddListener(OnClickCombine);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnClickBack);
    }

    public void HideUI()
    {
        panel.SetActive(false);
        recipesPanel.SetActive(false);
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
            if (inventoryManager.getQuantity(ing.item) < ing.amount)
            {
                canCraft = false;
                break;
            }
        }
        var resultText = resultSlot.Find("ResultText").GetComponent<TextMeshProUGUI>();

        if (!canCraft)
        {

            resultText.text = "<b><color=#FF3333>Not enough materials!</color></b>";
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
        if (inventoryManager.hasItem(currentRecipe.result))
        {
            inventoryManager.increaseQuantity(currentRecipe.result);
        }
        else
        {
            inventoryManager.AddInventoryItem(currentRecipe.result);
        }

        progressFill.fillAmount = 1f;
        resultText.text = "<b><color=#008001>Create successfull</color></b>";
        isCrafting = false;
    }
    public void OnClickBack()
    {   
        // Ẩn UI chi tiết recipe
       // panel.SetActive(false);

        // Hiển thị lại danh sách công thức
        recipesPanel.SetActive(true);

        // Dọn các slot nguyên liệu (nếu cần)
        foreach (var slot in currentSlots)
            Destroy(slot);
        currentSlots.Clear();

        Debug.Log("Back to recipe list");
    }

}


