using UnityEngine;
using PolyAndCode.UI;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.UI;
public class RecylableInventoryManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect recyclableScrollRect;
    [SerializeField]
    private int _dataLength;

    private List<ItemData> itemDataList = new List<ItemData>();
    private void Awake()
    {
        recyclableScrollRect.DataSource = this;
    }
    public int GetItemCount()
    {
        return itemDataList.Count;
    }

    public void SetCell(ICell cell, int index)
    {
        var item = cell as CellItemData;
        item.ConfigureCell(itemDataList[index], index);
    }

    private void Start()
    {
        List<ItemData> lstItem = new List<ItemData>();
        for (int i = 0; i <= 10; i++)
        {
            ItemData x = new ItemData();
            x.itemName = "Name_" + i;
            x.effect = "Decription of " + i;
            lstItem.Add(x);
        }
        SetLstItem(lstItem);
        recyclableScrollRect.ReloadData();
    }
    public void SetLstItem(List<ItemData> lstItem) {
        itemDataList = lstItem;
        }
}
