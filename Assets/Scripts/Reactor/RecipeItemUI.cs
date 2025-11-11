using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeItemUI : MonoBehaviour
{
    public TextMeshProUGUI recipeNameText;
    public Image recipeIcon;
    public Button selectButton;

    public CraftRecipe recipeData;

    
    public void Setup(CraftRecipe recipe)
    {
        recipeData = recipe;
        recipeNameText.text = recipe.result.itemName;
        recipeIcon.sprite = recipe.result.icon;
        // Gán sự kiện cho nút Select
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelectClicked);
    }

    public void OnSelectClicked()
    {
        Debug.Log("call onclick");
        // Gọi đến ReactorUIManager để hiển thị UI chi tiết của recipe này
        ReactorUIManager.Instance.ShowUI(recipeData);
    }
}