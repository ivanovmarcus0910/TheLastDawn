using NUnit.Framework;
using PolyAndCode.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class RecylableInventoryManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    private CanvasGroup canvasGroup;

    [SerializeField]
    RecyclableScrollRect recyclableScrollRect;
    [SerializeField]
    private int _dataLength;
    public GameObject menuGO;
    private List<ItemData> itemDataList = new List<ItemData>();
    private List<int> itemQuantityList = new List<int>();
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
        item.ConfigureCell(itemDataList[index], itemQuantityList[index], index);
    }

    private void Start()
    {
        canvasGroup = menuGO.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = menuGO.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (canvasGroup.alpha == 1f)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
    }
    public void AddInventoryItem(ItemData itemData)
    {
        itemDataList.Add(itemData);
        itemQuantityList.Add(1);

        recyclableScrollRect.ReloadData();
    }
    public bool hasItem(ItemData item)
    {
        return itemDataList.Contains(item);
    }
    public void increaseQuantity(ItemData item)
    {
        int id = 0;
        while (id < itemDataList.Count)
        {
            if (itemDataList[id] == item)
            {
                itemQuantityList[id] += 1;
                break;
            }
            id++;
        }
        recyclableScrollRect.ReloadData();
    
    }

    public int getQuantity(ItemData item)
    {
        int index = 0;
        while (index<itemDataList.Count)
        {
            if (itemDataList[index] == item)
            {
                return itemQuantityList[index];
            }
            index++;
        }
        return 0;
    }  
    public bool decreaseQuantity(ItemData item, int quantity)
    {
        int id = 0;
        while (id < itemDataList.Count)
        {
            if (itemDataList[id] == item)
            {
                itemQuantityList[id] -= quantity;
                return true;
            }
            id++;
        }
        recyclableScrollRect.ReloadData();
        return false;
    }
}
