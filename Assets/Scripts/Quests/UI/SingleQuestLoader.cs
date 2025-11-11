using UnityEngine;
using System.Linq; 

public class SingleQuestLoader : MonoBehaviour
{
    [Tooltip("Kéo DetailPanel (chứa script QuestDetailUI) vào đây")]
    public QuestDetailUI detailPanel;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        // Lấy CanvasGroup của chính panel này (do TabManager điều khiển)
        _canvasGroup = GetComponent<CanvasGroup>();
        if (detailPanel == null)
        {
            Debug.LogError("SingleQuestLoader: Chưa gán DetailPanel!", this);
        }
    }

    // Update được gọi mỗi frame
    void Update()
    {
        // Chỉ chạy khi panel này đang hiển thị (do TabManager bật lên)
        if (_canvasGroup == null || _canvasGroup.alpha == 0 || QuestManager.Instance == null)
        {
            return; // Không làm gì nếu panel đang ẩn
        }

        // 1. Tìm nhiệm vụ (Active) đầu tiên trong danh sách
        var activeQuest = QuestManager.Instance.GetVisibleQuests()
                              .FirstOrDefault(q => q.st.status == QuestStatus.Active);

        // 2. Nếu không có quest Active, tìm quest (Completed) đầu tiên (chờ trả)
        if (activeQuest.def == null)
        {
            activeQuest = QuestManager.Instance.GetVisibleQuests()
                              .FirstOrDefault(q => q.st.status == QuestStatus.Completed);
        }

        // 3. Nếu tìm thấy một quest (Active hoặc Completed)
        if (activeQuest.def != null)
        {
            // Gọi hàm Show() của QuestDetailUI để nó tự điền
            // (Script QuestDetailUI.cs đã có sẵn, chúng ta tái sử dụng nó)
            detailPanel.Show(activeQuest.def, activeQuest.st);
        }
        else
        {
            // 4. Nếu không có quest nào cả
            // Tự dọn dẹp các text
            detailPanel.titleText.text = "Không có nhiệm vụ";
            detailPanel.descriptionText.text = "Hãy khám phá thế giới để nhận nhiệm vụ mới.";
            detailPanel.objectivesText.text = "";
            detailPanel.rewardsText.text = "";

            // Ẩn các nút (vì hàm Show() không được gọi)
            if (detailPanel.acceptButton != null) detailPanel.acceptButton.gameObject.SetActive(false);
            if (detailPanel.turnInButton != null) detailPanel.turnInButton.gameObject.SetActive(false);
        }
    }
}