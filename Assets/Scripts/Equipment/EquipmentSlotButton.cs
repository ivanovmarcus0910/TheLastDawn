using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EquipmentSlotButton : MonoBehaviour
{
    // 1. Gán loại slot này trong Inspector
    public EquipmentSlot slotType;

    private Button slotButton;
    private Image slotImage;

    // Màu sắc để phân biệt Mặc / Cởi
    private Color unequippedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f); // Hơi xám, mờ
    private Color equippedColor = Color.white; // Rõ nét (mặc định)

    void Start()
    {
        slotButton = GetComponent<Button>();
        slotImage = GetComponent<Image>();

        // ▼▼▼ THAY ĐỔI QUAN TRỌNG (1) ▼▼▼
        // Tắt nút ngay khi bắt đầu. Không cho bấm
        // cho đến khi data được load xong.
        slotButton.interactable = false;

        // Gán sự kiện khi click
        slotButton.onClick.AddListener(OnSlotClicked);

        // Bắt đầu quá trình kiểm tra data
        InitializeVisuals();
    }

    // Hàm này chạy lúc bắt đầu để set màu đúng
    void InitializeVisuals()
    {
        // Chờ LoadDataManager load xong userInGame
        if (LoadDataManager.userInGame == null || LoadDataManager.userInGame.playerData == null)
        {
            // Nếu data chưa load, thử lại sau 0.5 giây (để đợi Firebase)
            Invoke(nameof(InitializeVisuals), 0.5f);
            return;
        }

        // ▼▼▼ THAY ĐỔI QUAN TRỌNG (2) ▼▼▼
        // Data đã load thành công! BẬT nút lên cho người chơi bấm.
        slotButton.interactable = true;

        // Kiểm tra trạng thái đã lưu
        bool isEquipped = LoadDataManager.userInGame.playerData.equipmentStatus.IsEquipped(slotType);
        UpdateVisuals(isEquipped);
    }

    // 4. HÀM CHÍNH: Chạy khi người dùng CLICK vào nút
    public void OnSlotClicked()
    {
        // 5. Kiểm tra an toàn (dù nút đã bị tắt, nhưng cẩn thận vẫn hơn)
        if (LoadDataManager.userInGame == null || LoadDataManager.userInGame.playerData == null)
        {
            // Ghi log chi tiết
            string errorMessage = $"[EquipmentSlotButton] Không thể thay đổi trang bị cho slot '{slotType}'. ";
            if (LoadDataManager.userInGame == null)
            {
                errorMessage += "Lý do: LoadDataManager.userInGame BỊ NULL.";
            }
            else
            {
                errorMessage += "Lý do: LoadDataManager.userInGame.playerData BỊ NULL.";
            }
            Debug.LogError(errorMessage);
            return;
        }


        // 6. Lấy data trang bị từ user (đang lưu trong RAM)
        var equipment = LoadDataManager.userInGame.playerData.equipmentStatus;

        // 7. Đảo ngược trạng thái
        bool currentState = equipment.IsEquipped(slotType);
        bool newState = !currentState; // (true -> false, false -> true)

        // 8. Cập nhật data TRONG BỘ NHỚ (RAM)
        equipment.SetSlot(slotType, newState);
        Debug.Log($"Đã {(newState ? "mặc" : "cởi")} slot: {slotType} (chưa lưu)");

        // 9. Cập nhật hình ảnh (mờ/rõ)
        UpdateVisuals(newState);

        // 10. (Không cần lưu vì ta đã thống nhất là chỉ lưu khi Quit)
    }

    // Hàm đổi màu
    private void UpdateVisuals(bool isEquipped)
    {
        if (slotImage == null) return;

        // Nếu đang mặc thì màu trắng, cởi ra thì màu xám mờ
        slotImage.color = isEquipped ? equippedColor : unequippedColor;
    }
}