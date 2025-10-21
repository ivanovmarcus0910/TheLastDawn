using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Crafting/Recipe")]
public class CraftRecipe : ScriptableObject
{
    [System.Serializable]
    public class Ingredient
    {
        public ItemData item;
        public int amount;
    }

    [Header("Input Materials")]
    public List<Ingredient> ingredients = new List<Ingredient>();

    [Header("Output Result")]
    public ItemData result;
    public int resultAmount = 1;
}
