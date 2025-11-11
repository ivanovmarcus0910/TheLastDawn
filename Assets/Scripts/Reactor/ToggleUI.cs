using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleUI : MonoBehaviour
{
    [Header("UI Panel")]
    [Tooltip("Kéo Panel (hoặc Canvas) bạn muốn điều khiển vào đây.")]
    public GameObject uiPanel;

    [Header("Phím Kích hoạt")]
    public Key toggleKey = Key.P;

    private bool isPanelActive = false;

    void Start()
    {
        if (uiPanel == null)
        {
            Debug.LogError("⚠️ Chưa gán UI Panel vào ToggleUI!");
            enabled = false;
            return;
        }

        // Ẩn UI khi bắt đầu game
        uiPanel.SetActive(false);
    }

    void Update()
    {
        // Kiểm tra phím có được nhấn trong khung hình này không
        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            // Thay đổi trạng thái UI
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        isPanelActive = !isPanelActive; // Đảo ngược trạng thái
        uiPanel.SetActive(isPanelActive);

    }
}