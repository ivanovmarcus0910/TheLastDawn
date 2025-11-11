using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DTO
{
    public class PlayerDataDTO
    {
        public int maxHealth;
        public int maxMana;
        public float moveSpeed;
        public int defense;
        public int baseDamage;
        public float jumpForce;

        // Convert từ PlayerData sang DTO để lưu
        public static PlayerDataDTO FromPlayerData(PlayerData data)
        {
            return new PlayerDataDTO
            {
                maxHealth = data.maxHealth,
                maxMana = data.maxMana,
                moveSpeed = data.moveSpeed,
                defense = data.defense,
                baseDamage = data.baseDamage,
                jumpForce = data.jumpForce
            };
        }

        // ✅ Convert ngược lại từ DTO → PlayerData (ScriptableObject)
        public PlayerData ToPlayerData()
        {
            PlayerData playerData = ScriptableObject.CreateInstance<PlayerData>();
            playerData.maxHealth = maxHealth;
            playerData.maxMana = maxMana;
            playerData.moveSpeed = moveSpeed;
            playerData.defense = defense;
            playerData.baseDamage = baseDamage;
            playerData.jumpForce = jumpForce;
            return playerData;
        }
        public string ToString()
        {
            return ($"Player Data DTO: {maxHealth} : {maxMana} : {moveSpeed} : {defense} : {baseDamage} : {jumpForce}");
        }
    }
}
