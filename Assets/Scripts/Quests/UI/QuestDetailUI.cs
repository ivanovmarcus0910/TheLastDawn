using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestDetailUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text objectivesText; // Ô text để liệt kê các mục tiêu
    public TMP_Text rewardsText; // Ô text để liệt kê phần thưởng
    public Button acceptButton; // Nút "Chấp nhận"
    public Button turnInButton; // Nút "Trả Quest"
    public Button closeButton; // 

    private QuestDefinition currentDef;
    private QuestManager.QuestRuntimeState currentState;

    private void Awake()
    {
        // Gắn sự kiện cho các nút bấm (chỉ chạy 1 lần)
        if (acceptButton != null) acceptButton.onClick.AddListener(OnAcceptClicked);
        if (turnInButton != null) turnInButton.onClick.AddListener(OnTurnInClicked);
        if (closeButton != null) closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Show(QuestDefinition def, QuestManager.QuestRuntimeState st)
    {
        currentDef = def;
        currentState = st;

        // 1. Cập nhật Text
        titleText.text = def.title;
        descriptionText.text = def.description;
        // Gọi hàm helper để xây dựng chuỗi text cho mục tiêu và phần thưởng
        objectivesText.text = BuildObjectivesText(def, st);
        rewardsText.text = BuildRewardsText(def);

        // 2. Cập nhật trạng thái các Nút bấm
        // Hiện nút "Accept" NẾU quest đang 'Available'
        acceptButton.gameObject.SetActive(st.status == QuestStatus.Available);
        // Hiện nút "TurnIn" NẾU quest đang 'Completed' VÀ quest không 'autoTurnIn'
        turnInButton.gameObject.SetActive(st.status == QuestStatus.Completed && !def.autoTurnIn);

        // 3. Hiện toàn bộ Panel chi tiết
        gameObject.SetActive(true);
    }

    private string BuildObjectivesText(QuestDefinition def, QuestManager.QuestRuntimeState st)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < def.objectives.Count; i++)
        {
            var o = def.objectives[i];
            var rt = st.objectives[i];
            // Ví dụ: "- Tiêu diệt SandWraith (5/10)"
            sb.AppendLine($"- {o.description} ({o.GetProgressText(rt)})");
        }
        return sb.ToString();
    }

    private string BuildRewardsText(QuestDefinition def)
    {
        var sb = new StringBuilder();
        if (def.rewardExperience > 0) sb.AppendLine($"XP: {def.rewardExperience}");
        if (def.rewardItems != null && def.rewardItems.Length > 0)
        {
            foreach (var it in def.rewardItems)
                sb.AppendLine($"{it.itemId} x{it.amount}");
        }
        return sb.ToString();
    }


    private void OnAcceptClicked()
    {
        if (currentDef == null) return;
        // Gọi QuestManager để chấp nhận quest
        QuestManager.Instance.AcceptQuest(currentDef.QuestId);

        // Cập nhật lại trạng thái (vì nó vừa đổi từ Available -> Active)
        UpdateCurrentState();
        // Tải lại UI với trạng thái mới
        Show(currentDef, currentState);
    }


    private void OnTurnInClicked()
    {
        if (currentDef == null) return;
        // Gọi QuestManager để trả quest
        QuestManager.Instance.TurnIn(currentDef.QuestId);

        // Cập nhật lại trạng thái (vì nó vừa đổi từ Completed -> TurnedIn)
        UpdateCurrentState();
        // Tải lại UI với trạng thái mới
        Show(currentDef, currentState);
    }


    private void UpdateCurrentState()
    {
        foreach (var pair in QuestManager.Instance.GetVisibleQuests())
        {
            if (pair.def.QuestId == currentDef.QuestId)
            {
                currentState = pair.st;
                return;
            }
        }
    }
}