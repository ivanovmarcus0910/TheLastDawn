using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QuestProgress
{
    public int questID; 
    public bool isCompleted;
    public int currentAmount;

    public QuestProgress(int id)
    {
        questID = id;
        isCompleted = false;
        currentAmount = 0;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Nhiệm vụ")]
    public List<QuestData> allQuests;

    public Dictionary<int, QuestProgress> playerQuestProgress;

    private QuestData _currentActiveQuest;
    private QuestProgress _currentQuestProgress;
    public QuestData GetCurrentActiveQuest()
    {
        return _currentActiveQuest;
    }
    public QuestProgress GetCurrentQuestProgress()
    {
        return _currentQuestProgress;
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerQuestProgress = new Dictionary<int, QuestProgress>();

        if (allQuests.Count > 0)
        {
            StartQuest(allQuests[0].questID);
        }
    }

    public void StartQuest(int questID)
    {
        _currentActiveQuest = allQuests.Find(q => q.questID == questID);
        if (_currentActiveQuest == null)
        {
            Debug.LogError($"Không tìm thấy QuestData với ID: {questID}");
            return;
        }

        _currentQuestProgress = new QuestProgress(questID); // (Dùng ID (int))
        playerQuestProgress[questID] = _currentQuestProgress;

        Debug.Log($"Đã nhận nhiệm vụ mới: {_currentActiveQuest.title}");
    }

    public void UpdateQuestProgress(ObjectiveType type, string id, int amount = 1)
    {
        if (_currentActiveQuest == null || _currentQuestProgress.isCompleted)
        {
            return;
        }

        if (_currentActiveQuest.objectiveType == type && _currentActiveQuest.targetID == id)
        {
            _currentQuestProgress.currentAmount += amount;
            Debug.Log($"Tiến độ nhiệm vụ: {_currentQuestProgress.currentAmount} / {_currentActiveQuest.targetAmount}");

            if (_currentQuestProgress.currentAmount >= _currentActiveQuest.targetAmount)
            {
                CompleteQuest();
            }
        }
    }

    private void CompleteQuest()
    {
        if (_currentActiveQuest == null) return;

        _currentQuestProgress.isCompleted = true;
        Debug.Log($"Hoàn thành nhiệm vụ: {_currentActiveQuest.title}!");

        // ... (Code trao thưởng giữ nguyên) ...
        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory != null && _currentActiveQuest.rewardItem != null)
        {
            playerInventory.AddItem(_currentActiveQuest.rewardItem, _currentActiveQuest.rewardItemAmount);
        }

        // Bắt đầu nhiệm vụ tiếp theo (vẫn dùng questID (int))
        if (_currentActiveQuest.nextQuest != null)
        {
            StartQuest(_currentActiveQuest.nextQuest.questID);
        }
        else
        {
            _currentActiveQuest = null;
            _currentQuestProgress = null;
            Debug.Log("Đã hoàn thành tất cả nhiệm vụ!");
        }

        // === LƯU DATABASE ===
        // Bây giờ 'playerQuestProgress' (Dictionary<int, ...>)
        // sẽ dễ dàng được chuyển đổi để lưu vào database của bạn.

    }
}