using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Create New Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int price;
    public Sprite icon;
    public GameObject itemPrefab;  
    [TextArea] public string effect; 
    public bool canBeSold = true;   
}
