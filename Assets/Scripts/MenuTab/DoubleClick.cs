using UnityEngine;
using UnityEngine.UI;

public class DoubleClick : MonoBehaviour
{
    private Button button;
    private float lastClickTime = 0f;
    public float doubleClickDelay = 0.3f; // thời gian tối đa giữa 2 lần nhấn
    private CellItemData cellItemData;
    private HandleUseItem handleUseItem;
    private void Awake()
    {
        // Lấy component Button nằm cùng GameObject
        button = GetComponent<Button>();
        cellItemData = GetComponent<CellItemData>();
    }

    private void Start()
    {
        // Gán sự kiện click của Button sang hàm riêng
        button.onClick.AddListener(OnButtonClicked);
        handleUseItem = FindFirstObjectByType<HandleUseItem>();

    }

    private void OnButtonClicked()
    {
        if (Time.time - lastClickTime < doubleClickDelay)
        {
            Debug.Log("🚀 Double Click!");
            OnDoubleClick();
        }

        lastClickTime = Time.time;
    }

    private void OnDoubleClick()
    {
        handleUseItem.UseItem(cellItemData.itemData);
        // 🔥 Hành động thực tế khi double click
        Debug.Log("💥 Đã chạy hàm double click! trên object : " + cellItemData.itemData.itemName);
    }
}
