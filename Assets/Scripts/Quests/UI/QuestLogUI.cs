using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quản lý chính cho TOÀN BỘ cửa sổ Quest Log (bảng lớn).
/// Gắn component này vào một GameObject gốc (ví dụ: QuestLogManager) trên Canvas.
/// </summary>
public class QuestLogUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot; // Panel gốc (để bật/tắt)
    public Button openButton;    // Nút trên HUD (nếu có) để mở panel này
    public Button closeButton;   // Nút 'X' để đóng panel
    public TMP_Dropdown filterDropdown; // (Tùy chọn) Dropdown để lọc quest

    [Header("List Configuration")]
    public RectTransform listContainer; // Vùng Content của ScrollView nơi sẽ chứa các hàng
    public GameObject entryPrefab; // Prefab của một hàng (phải có script QuestEntryUI)

    [Header("Detail Panel")]
    public QuestDetailUI detailPanel; // Kéo Panel chi tiết (có script QuestDetailUI) vào đây

    // Danh sách nội bộ để lưu các GameObject đã được tạo ra
    private List<GameObject> instantiatedEntries = new List<GameObject>();

    // === BIẾN MỚI ĐỂ KIỂM TRA ===
    private CanvasGroup _panelCanvasGroup;
    private bool _listNeedsRefresh = true;

    private void Start()
    {
        // Lấy CanvasGroup của panel gốc (PanelNhiemVu)
        if (panelRoot != null)
        {
            _panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
            if (_panelCanvasGroup == null)
            {
                Debug.LogError("LỖI: QuestLogUI cần PanelNhiemVu phải có component CanvasGroup!", this);
            }
        }

        // Gắn sự kiện cho các nút bấm (những nút này đã được gán = None)
        if (openButton != null) openButton.onClick.AddListener(TogglePanel);
        if (closeButton != null) closeButton.onClick.AddListener(() => SetPanelVisible(false));
        if (filterDropdown != null) filterDropdown.onValueChanged.AddListener(OnFilterChanged);

        // === ĐÃ XÓA DÒNG SetPanelVisible(false); ===

        // Ẩn detail panel lúc đầu
        if (detailPanel != null) detailPanel.gameObject.SetActive(false);

        // Tải danh sách lần đầu
        RefreshList();
    }

    private void Update()
    {
        // Cho phép người chơi Bật/Tắt panel bằng phím Q (ví dụ)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // (Lưu ý: Đoạn code này sẽ xung đột với TabManager của bạn,
            // bạn nên xóa nó VÀ ngắt kết nối open/close button trong Inspector)
            // TogglePanel(); 
        }

        // === LOGIC UPDATE ĐÃ SỬA ===
        if (_panelCanvasGroup != null)
        {
            // Nếu panel đang được hiển thị (alpha > 0)
            if (_panelCanvasGroup.alpha > 0)
            {
                // Chỉ refresh 1 LẦN khi nó vừa hiện ra
                if (_listNeedsRefresh)
                {
                    RefreshList();
                    _listNeedsRefresh = false;
                }
            }
            else
            {
                // Khi panel bị ẩn đi (alpha = 0), đánh dấu là "cần refresh" cho lần mở sau
                _listNeedsRefresh = true;
            }
        }
    }

    // === CÁC HÀM NÀY SẼ KHÔNG ĐƯỢC DÙNG NỮA VÌ TABMANAGER ĐÃ KIỂM SOÁT ===
    // (Chúng ta vẫn giữ chúng ở đây phòng trường hợp bạn gỡ TabManager)
    private void TogglePanel()
    {
        SetPanelVisible(!(panelRoot != null && panelRoot.activeSelf));
    }
    private void SetPanelVisible(bool visible)
    {
        if (panelRoot != null) panelRoot.SetActive(visible);
        if (visible)
        {
            RefreshList();
            if (detailPanel != null) detailPanel.gameObject.SetActive(false);
        }
    }
    // === KẾT THÚC CÁC HÀM KHÔNG DÙNG ===


    private void OnFilterChanged(int idx)
    {
        RefreshList();
    }

    public void RefreshList()
    {
        if (listContainer == null || entryPrefab == null || QuestManager.Instance == null) return;

        // 1. Xóa tất cả các hàng (entry) cũ
        foreach (var go in instantiatedEntries) Destroy(go);
        instantiatedEntries.Clear();

        int filter = filterDropdown != null ? filterDropdown.value : 0;

        // 2. Lấy tất cả quest từ QuestManager và tạo hàng mới
        foreach (var (def, st) in QuestManager.Instance.GetVisibleQuests())
        {
            if (!MatchFilter(st.status, filter)) continue;

            var go = Instantiate(entryPrefab, listContainer);
            var entry = go.GetComponent<QuestEntryUI>();
            if (entry != null)
            {
                entry.Setup(def, st, this);
            }
            instantiatedEntries.Add(go);
        }
    }

    private bool MatchFilter(QuestStatus status, int filter)
    {
        switch (filter)
        {
            case 1: return status == QuestStatus.Available;
            case 2: return status == QuestStatus.Active;
            case 3: return status == QuestStatus.Completed;
            default: return true;
        }
    }

    public void SelectQuest(QuestDefinition def, QuestManager.QuestRuntimeState st)
    {
        if (detailPanel != null) detailPanel.Show(def, st);
    }
}