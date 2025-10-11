using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public ItemData item;
    [Range(0f, 1f)]
    public float dropChance = 0.2f;
}

[CreateAssetMenu(fileName = "NewLootTable", menuName = "Loot/Create New Loot Table")]
public class LootTable : ScriptableObject
{
    public LootEntry[] lootItems;
}

