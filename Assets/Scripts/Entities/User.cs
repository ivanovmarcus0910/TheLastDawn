using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class User 
{
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string Name { get; set; }
    public List<ItemData> itemDataList { get; set; }
    public List<int> itemQuantityList { get; set; }
    public PlayerData playerData { get; set; }
    public int SiC { get; set; }

    public User()
    {
    }

    public User(string name, List<ItemData> itemDataList, List<int> itemQuantityList, PlayerData playerData, int siC)
    {
        this.Name = name;
        this.itemDataList = itemDataList;
        this.itemQuantityList = itemQuantityList;
        this.playerData = playerData;
        this.SiC = siC;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
