using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class QuestTriggerArea : MonoBehaviour
{
    [Tooltip("ID của khu vực. PHẢI KHỚP với 'areaId' trong ReachAreaObjectiveDefinition")]
    public string areaId;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem 'areaId' đã được gán chưa và đối tượng va chạm có tag "Player" không
        if (!string.IsNullOrEmpty(areaId) && other.CompareTag("Player"))
        {
            // Bắn sự kiện lên Trung tâm
            GameEventHub.AreaEntered(areaId);

            // Destroy(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(areaId) && other.CompareTag("Player"))
        {
            GameEventHub.AreaEntered(areaId);
            // Destroy(this);
        }
    }
}