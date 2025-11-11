using Assets.Scripts.DTO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class User 
{
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string Name { get; set; }
    public List<ItemDataDTO> itemDataList { get; set; }
    public List<int> itemQuantityList { get; set; }
    public PlayerDataDTO playerData { get; set; }
    public int SiC { get; set; }
    public int currentMapIndex { get; set; }

    public User()
    {
    }

    public User(string name, List<ItemDataDTO> itemDataList, List<int> itemQuantityList, PlayerDataDTO playerData, int siC, int currentMap)
    {
        this.Name = name;
        this.itemDataList = itemDataList;
        this.itemQuantityList = itemQuantityList;
        this.playerData = playerData;
        this.SiC = siC;
        this.currentMapIndex = currentMap;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
