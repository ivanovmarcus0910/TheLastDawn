using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

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

    [Header("Master List")]
    [Tooltip("Kéo TẤT CẢ các file QuestData của game vào đây THEO ĐÚNG THỨ TỰ")]
    public List<QuestData> allQuests; 

    public Dictionary<int, QuestProgress> playerQuestProgress;

   
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        playerQuestProgress = new Dictionary<int, QuestProgress>();

    
        if (allQuests.Count > 0)
        {
            StartQuest(allQuests[0].questID);
        }
    }

    public void StartQuest(int questID)
    {
        if (playerQuestProgress.ContainsKey(questID)) return;
        QuestData questToStart = allQuests.Find(q => q.questID == questID);
        if (questToStart == null) return;

        QuestProgress newProgress = new QuestProgress(questID);
        playerQuestProgress[questID] = newProgress;
        Debug.Log($"Đã nhận nhiệm vụ mới: {questToStart.title}");
    }

    public void UpdateQuestProgress(ObjectiveType type, string id, int amount = 1)
    {
        if (playerQuestProgress.Count == 0) return;

        foreach (QuestProgress progress in playerQuestProgress.Values.ToList())
        {
            if (progress.isCompleted) continue;
            QuestData quest = allQuests.Find(q => q.questID == progress.questID);
            if (quest == null) continue;

            if (quest.objectiveType == type && quest.targetID == id)
            {
                progress.currentAmount += amount;
                Debug.Log($"Tiến độ '{quest.title}': {progress.currentAmount} / {quest.targetAmount}");

                if (progress.currentAmount >= quest.targetAmount)
                {
                    CompleteQuest(quest.questID);
                }
            }
        }
    }

    // === HÀM NÀY ĐÃ ĐƯỢC NÂNG CẤP ĐỂ "DUYỆT LIST" ===
    private void CompleteQuest(int questID)
    {
        QuestData quest = allQuests.Find(q => q.questID == questID);
        if (quest == null) return;

        if (!playerQuestProgress.ContainsKey(questID)) return;
        QuestProgress progress = playerQuestProgress[questID];
        if (progress.isCompleted) return;

        progress.isCompleted = true;
        Debug.Log($"Hoàn thành nhiệm vụ: {quest.title}!");


        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();
        PlayerBase playerBase = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBase>(); 
        if (playerInventory != null && quest.rewardItem != null)
        {
            playerInventory.AddItem(quest.rewardItem, quest.rewardItemAmount);
            playerBase.updateEXP(quest.rewardExp);
        }
      
        int completedQuestIndex = allQuests.FindIndex(q => q.questID == questID);

        
        if (completedQuestIndex != -1 && (completedQuestIndex + 1) < allQuests.Count)
        {
           
            QuestData nextQuest = allQuests[completedQuestIndex + 1];

            
            StartQuest(nextQuest.questID);
            Debug.Log($"Đã tự động bắt đầu nhiệm vụ tiếp theo: {nextQuest.title}");
        }
        else
        {
           
            Debug.Log("Đã hoàn thành tất cả nhiệm vụ trong chuỗi!");
           
        }

    }
    public List<QuestProgress> GetPlayerQuests()
    {
        return playerQuestProgress.Values.ToList();
    }

    public QuestData GetQuestData(int questID)
    {
        return allQuests.Find(q => q.questID == questID);
    }
}