using UnityEngine;
using UnityEngine.InputSystem;

public class DogCageController : MonoBehaviour
{
    [Header("References")]
    public GameObject dog;                   // Con chó trong lồng
    public GameObject cageClosed;            // Sprite lồng đóng
    public GameObject cageOpen;              // Sprite lồng mở
    public Transform player;                 // Player reference
    public HintUI hintUI;                    // Hiển thị UI gợi ý
    public RecylableInventoryManager inventoryManager; // 🔑 Tham chiếu tới Inventory
    public ItemData keyItem;                 // 🔑 Item cần có để mở lồng
    public Transform petTeleportPoint;  // 🧭 điểm teleport gần player


    private bool isUnlocked = false;
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Nếu có khóa trong túi thì hiển thị gợi ý mở lồng
            if (inventoryManager != null && inventoryManager.hasItem(keyItem))
                hintUI?.ShowHint("Press E to rescue");
            else
                hintUI?.ShowHint("You need a key to unlock this cage");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hintUI?.HideHint();
        }
    }

    void Update()
    {
        if (playerInRange && !isUnlocked && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryUnlockCage();
        }
    }

    void TryUnlockCage()
    {
        // ✅ Kiểm tra Inventory có KeyItem chưa
        if (inventoryManager == null )
        {
            Debug.LogWarning("⚠️ InventoryManager hoặc KeyItem chưa được gán!");
            return;
        }

        if (!inventoryManager.hasItem(keyItem))
        {
            Debug.Log("🔒 Bạn chưa có chìa khóa!");
            hintUI?.ShowHint("You need a key to unlock this cage");
            return;
        }

        // Nếu có key thì mở lồng
        UnlockCage();
    }

    void UnlockCage()
    {
        Debug.Log("🔓 Cage unlocked!");
        isUnlocked = true;
        hintUI?.HideHint();

        // 🔓 Mở lồng
        if (cageClosed != null) cageClosed.SetActive(false);
        if (cageOpen != null) cageOpen.SetActive(true);

        // 🐶 Animation Happy
        Animator anim = dog.GetComponent<Animator>();
        anim?.SetTrigger("Rescued");

        // 🦴 Thêm script follow
        PetFollower follow = dog.AddComponent<PetFollower>();
        follow.target = player;
        follow.teleportPoint = petTeleportPoint;


        dog.transform.SetParent(null);

        // (Tuỳ chọn) Trừ 1 key sau khi sử dụng
        inventoryManager.decreaseQuantity(keyItem, 1);
    }
}
