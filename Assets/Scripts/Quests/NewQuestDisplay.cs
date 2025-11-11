using UnityEngine;
using TMPro; 


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

        
        QuestData quest = QuestManager.Instance.GetCurrentActiveQuest();
        QuestProgress progress = QuestManager.Instance.GetCurrentQuestProgress();

        
        if (quest != null && progress != null)
        {
           
            titleText.text = quest.title;
            descriptionText.text = quest.description;

            
            objectiveText.text = $"- {quest.targetID}: {progress.currentAmount} / {quest.targetAmount}";

            
            rewardText.text = $"Thưởng: {quest.rewardExp} EXP";
            if (quest.rewardItem != null)
            {
                rewardText.text += $"\n- {quest.rewardItemAmount} x {quest.rewardItem.itemName}";
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