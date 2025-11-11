using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Assets.Scripts.DTO
{
    [System.Serializable]
    public class ItemDataDTO
    {
        public string itemName;
        public int price;
        public string effect;
        public bool canBeSold;
        public string iconName;    // Sprite name
        public string prefabName;  // Prefab name

        public static ItemDataDTO FromItemData(ItemData data)
        {
            return new ItemDataDTO
            {
                itemName = data.itemName,
                price = data.price,
                effect = data.effect,
                canBeSold = data.canBeSold,
                iconName = data.icon != null ? data.icon.name : "",
                prefabName = data.itemPrefab != null ? data.itemPrefab.name : ""
            };
        }
        public ItemData ToItemData()
        {
            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = itemName;
            item.price = price;
            item.effect = effect;
            item.canBeSold = canBeSold;
            Sprite[] sprites = Resources.LoadAll<Sprite>($"Icons/{iconName}");
            item.icon = System.Array.Find(sprites, s => s.name == iconName);
            item.itemPrefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
            return item;
        }
    }

}
