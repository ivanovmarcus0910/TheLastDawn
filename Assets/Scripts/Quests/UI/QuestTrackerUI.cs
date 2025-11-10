using System.Text;
using UnityEngine;
using TMPro; // Cần thư viện TextMeshPro

public class QuestTrackerUI : MonoBehaviour
{
    // Kéo ô TextMeshPro (TMP_Text) của bạn vào đây trong Inspector
    public TMP_Text displayText;

    private void Update()
    {
        // Nếu không có ô text hoặc QuestManager chưa sẵn sàng, thì không làm gì cả
        if (displayText == null || QuestManager.Instance == null) return;

        // StringBuilder hiệu quả hơn việc cộng chuỗi (string + string)
        var sb = new StringBuilder();

        // 1. Lấy tất cả quest đang hiển thị từ QuestManager
        foreach (var (def, st) in QuestManager.Instance.GetVisibleQuests())
        {
            // 2. CHỈ hiển thị các quest đang "Active" hoặc "Completed" (chờ trả)
            if (st.status == QuestStatus.Active || st.status == QuestStatus.Completed)
            {
                // Thêm tên quest
                sb.AppendLine($"• {def.title} ({st.status})");

                // 3. Duyệt qua TỪNG objective con của quest này
                for (int i = 0; i < def.objectives.Count; i++)
                {
                    var o = def.objectives[i]; // Định nghĩa (để lấy mô tả)
                    var rt = st.objectives[i]; // Trạng thái (để lấy tiến độ)

                    // Lấy text tiến độ (ví dụ: "5/10")
                    var progress = o.GetProgressText(rt);
                    // Thêm dòng mô tả objective
                    sb.AppendLine($"  - {o.description}: {progress}");
                }
            }
        }

        // 4. Gán toàn bộ chuỗi đã xây dựng vào ô text
        displayText.text = sb.ToString();
    }
}