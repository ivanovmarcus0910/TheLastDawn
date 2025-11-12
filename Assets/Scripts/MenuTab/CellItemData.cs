using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using PolyAndCode.UI;
public class CellItemData : MonoBehaviour, ICell
{
    public Text nameLabel;
    public Text quantityLabel;
    public Text decriptionLabel;
    public Image icon;
    public ItemData itemData;
    private int cellIndex;
    public void ConfigureCell (ItemData itemData, int quantity, int cellIndex)
    {
        this.itemData = itemData;
        this.cellIndex = cellIndex;
        this.icon.sprite = itemData.icon;
        nameLabel.text = itemData.itemName;
        quantityLabel.text = quantity.ToString();
        decriptionLabel.text = itemData.effect;
    }    

}
