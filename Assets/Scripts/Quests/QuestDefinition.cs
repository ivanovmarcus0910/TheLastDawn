using System;
using System.Collections.Generic;
using UnityEngine;

// Định nghĩa các 'enum' (bộ hằng số) để dễ quản lý trạng thái
public enum QuestScope { Global = 0, Map = 1 }


/// Các trạng thái của một nhiệm vụ.
public enum QuestStatus { Locked = 0, Available = 1, Active = 2, Completed = 3, TurnedIn = 4 }


[CreateAssetMenu(menuName = "Quests/Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    // ID nội bộ duy nhất cho quest
    [SerializeField] private string questId = System.Guid.NewGuid().ToString("N");
    public string title; // Tên quest
    [TextArea] public string description; // Mô tả chi tiết

    [Header("Scoping")]
    public QuestScope scope = QuestScope.Map;
    [Tooltip("Nếu scope là Map, mapId phải khớp với tên Scene (ví dụ: 'Desert')")]
    public string mapId = "";

    [Header("Flow")]
    public bool autoAcceptOnMapEnter = false; // Tự động nhận quest khi vào map?
    public bool autoTurnIn = false; // Tự động trả quest (và nhận thưởng) khi hoàn thành?
    public bool repeatable = false; // Có thể làm lại quest này không?
    public float timeLimitSeconds = 0f; // Giới hạn thời gian (0 = không giới hạn)
    public QuestDefinition nextQuestOnComplete; // (Nâng cao) Quest tiếp theo trong chuỗi

    [Header("Data")]
    // Đây là phần QUAN TRỌNG:
    // Bạn sẽ kéo các file ObjectiveDefinition (ví dụ: Kill_SandWraith, Collect_WhiteBone)
    // vào danh sách (list) này trong cửa sổ Inspector của Unity.
    public List<ObjectiveDefinition> objectives = new List<ObjectiveDefinition>();

    [Header("Rewards (ví dụ)")]
    public int rewardExperience = 0; // Lượng XP thưởng
    public RewardItem[] rewardItems; // Mảng các vật phẩm thưởng

    // Hàm public (chỉ đọc) để lấy ID
    public string QuestId => questId;
}

