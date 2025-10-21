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

    private ItemData itemData;
    private int cellIndex;
    public void ConfigureCell (ItemData itemData, int cellIndex)
    {
        this.itemData = itemData;
        this.cellIndex = cellIndex;
        nameLabel.text = itemData.itemName;
        quantityLabel.text = "1";
        decriptionLabel.text = itemData.effect;
    }    

}
