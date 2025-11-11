using UnityEngine;
using TMPro;
using System.Linq; 

[RequireComponent(typeof(CanvasGroup))]
public class NewQuestDisplay : MonoBehaviour
{
    [Header("Giao diện (Kéo từ DetailPanel)")]
    public TMP_Text titleText;       // Kéo Text Tiêu đề vào
    public TMP_Text descriptionText; // Kéo DescriptionText vào
    public TMP_Text objectiveText;   // Kéo Text Mục tiêu vào
    public TMP_Text rewardText;      // Kéo RewardsText vào

    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }


    void Update()
    {

        if (_canvasGroup.alpha == 0 || QuestManager.Instance == null)
        {
            return;
        }

        
        var allPlayerQuests = QuestManager.Instance.GetPlayerQuests();

        
        QuestProgress activeProgress = allPlayerQuests.FirstOrDefault(q => !q.isCompleted);

        QuestData questToShow = null;
        QuestProgress progressToShow = null;

        if (activeProgress != null)
        {
           
            questToShow = QuestManager.Instance.GetQuestData(activeProgress.questID);
            progressToShow = activeProgress;
        }
        else
        {
           
            QuestProgress completedProgress = allPlayerQuests.FirstOrDefault(q => q.isCompleted);
            if (completedProgress != null)
            {
                questToShow = QuestManager.Instance.GetQuestData(completedProgress.questID);
                progressToShow = completedProgress;
            }
        }

        
        if (questToShow != null && progressToShow != null)
        {
            
            if (progressToShow.isCompleted)
            {
                titleText.text = questToShow.title + " (ĐÃ HOÀN THÀNH)";
            }
            else
            {
                titleText.text = questToShow.title;
            }

            descriptionText.text = questToShow.description;

            
            if (!progressToShow.isCompleted)
            {
                objectiveText.text = $"- {questToShow.targetID}: {progressToShow.currentAmount} / {questToShow.targetAmount}";
            }
            else
            {
                objectiveText.text = "Đã xong mục tiêu!";
            }

            
            rewardText.text = $"Thưởng: {questToShow.rewardExp} EXP";
            if (questToShow.rewardItem != null)
            {
                
                rewardText.text += $"\n- {questToShow.rewardItemAmount} x {questToShow.rewardItem.itemName}";
            }
        }
        else
        {
            
            titleText.text = "Không có nhiệm vụ";
            descriptionText.text = "Hãy khám phá thế giới để nhận nhiệm vụ mới.";
            objectiveText.text = "";
            rewardText.text = "";
        }
    }
}