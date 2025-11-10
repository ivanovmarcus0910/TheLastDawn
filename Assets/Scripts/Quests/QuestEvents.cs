using System;
using UnityEngine;

public interface IQuestEvent { }

// Sự kiện được bắn ra khi một kẻ thù bị tiêu diệt.
[Serializable]
public struct EnemyKilledEvent : IQuestEvent
{
    public string enemyId; // ID của kẻ thù 
    public Vector3 position; // Vị trí kẻ thù chết
    public int level; // Cấp độ của kẻ thù (nếu cần)
}

// Sự kiện được bắn ra khi người chơi nhặt vật phẩm.
[Serializable]
public struct ItemCollectedEvent : IQuestEvent
{
    public string itemId; // ID của vật phẩm 
    public int amount; // Số lượng vừa nhặt
}

// Sự kiện được bắn ra khi người chơi đi vào một khu vực (Trigger).
[Serializable]
public struct AreaEnteredEvent : IQuestEvent
{
    public string areaId; // ID của khu vực 
}

// Sự kiện được bắn ra khi người chơi tương tác với một đối tượng.
[Serializable]
public struct InteractedEvent : IQuestEvent
{
    public string interactableId; // ID của đối tượng
    public string action; // loại tương tác
}

[Serializable]
public struct RewardGrantedEvent : IQuestEvent
{
    public string questId;
    public int experience; // Lượng XP được thưởng
    public RewardItem[] items; // Mảng các vật phẩm được thưởng
}

[Serializable]
public struct RewardItem
{
    public string itemId;
    public int amount;
}


public static class GameEventHub
{
    public static event Action<IQuestEvent> OnEvent;

    public static void Publish(IQuestEvent evt)
    {
        OnEvent?.Invoke(evt);
    }

    // Các hàm "lối tắt" (shortcut) tiện lợi cho các script khác gọi q

    public static void EnemyKilled(string enemyId, Vector3 pos, int level = 1)
        => Publish(new EnemyKilledEvent { enemyId = enemyId, position = pos, level = level });

    public static void ItemCollected(string itemId, int amount)
        => Publish(new ItemCollectedEvent { itemId = itemId, amount = amount });

    public static void AreaEntered(string areaId)
        => Publish(new AreaEnteredEvent { areaId = areaId });

    public static void Interacted(string id, string action = "")
        => Publish(new InteractedEvent { interactableId = id, action = action });

    public static void RewardGranted(string questId, int exp, RewardItem[] items)
        => Publish(new RewardGrantedEvent { questId = questId, experience = exp, items = items });
}