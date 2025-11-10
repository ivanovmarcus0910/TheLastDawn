using UnityEngine;
using UnityEngine.UI; // Cần thư viện UI
using TMPro; // Cần thư viện TextMeshPro


public class QuestEntryUI : MonoBehaviour
{
    public TMP_Text titleText; // Ô text hiển thị tên quest
    public TMP_Text statusText; // Ô text hiển thị trạng thái (Active, Completed...)
    public Button selectButton; // Nút bấm (toàn bộ hàng) để chọn

    // Biến nội bộ để lưu quest này là gì
    private QuestDefinition questDef;
    private QuestManager.QuestRuntimeState questState;
    private QuestLogUI parent; // Tham chiếu đến script cha (QuestLogUI)

    public void Setup(QuestDefinition def, QuestManager.QuestRuntimeState st, QuestLogUI parentUI)
    {
        questDef = def;
        questState = st;
        parent = parentUI; // Lưu lại tham chiếu đến script cha

        // Cập nhật text
        titleText.text = def.title;
        statusText.text = st.status.ToString();

        // Thiết lập sự kiện cho nút bấm
        selectButton.onClick.RemoveAllListeners(); // Xóa listener cũ (nếu có)
        selectButton.onClick.AddListener(OnSelect); // Thêm listener mới
    }

    private void OnSelect()
    {
        // Báo cho script cha (QuestLogUI) rằng:
        // "Người chơi vừa chọn tôi, hãy hiển thị chi tiết của quest [questDef]"
        if (parent != null && questDef != null)
            parent.SelectQuest(questDef, questState);
    }
}