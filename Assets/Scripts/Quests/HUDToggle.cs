// Assets/Scripts/UI/HUDToggle.cs
using UnityEngine;
using UnityEngine.UI;

// Gắn script này vào ToggleHUDButton (button icon)
public class HUDToggle : MonoBehaviour
{
    [Tooltip("Kéo 'TextHUDNhiemVu' (GameObject hiển thị nhiệm vụ) có CanvasGroup vào đây")]
    public CanvasGroup hudContentGroup;

    private Button _toggleButton;
    private bool isVisible = false;

    void Start()
    {
        _toggleButton = GetComponent<Button>();
        if (_toggleButton != null)
            _toggleButton.onClick.AddListener(ToggleHUD);

        if (hudContentGroup == null)
        {
            Debug.LogWarning("[HUDToggle] Hud Content Group chưa gán. Kéo CanvasGroup của TextHUDNhiemVu vào trường này.");
            return;
        }

        // Nếu hudContentGroup trỏ vào chính object chứa nút (sai gán), thì log và không ẩn chính nút
        if (hudContentGroup.gameObject == this.gameObject)
        {
            Debug.LogWarning("[HUDToggle] Hud Content Group trỏ vào chính Toggle button. Vui lòng gán CanvasGroup của panel hiển thị nhiệm vụ (TextHUDNhiemVu).");
            // giữ nút visible; không gọi SetVisibility trên chính button
            isVisible = true;
            return;
        }

        // Ẩn nội dung lúc bắt đầu
        SetVisibility(false);
    }

    public void ToggleHUD()
    {
        if (hudContentGroup == null) return;
        isVisible = !isVisible;
        SetVisibility(isVisible);
    }

    public void SetVisibility(bool visible)
    {
        if (hudContentGroup == null) return;
        hudContentGroup.alpha = visible ? 1f : 0f;
        hudContentGroup.interactable = visible;
        hudContentGroup.blocksRaycasts = visible;
    }
}