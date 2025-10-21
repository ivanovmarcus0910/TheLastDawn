using UnityEngine;
using UnityEngine.InputSystem;

public class DogCageController : MonoBehaviour
{
    [Header("References")]
    public GameObject dog;            // Con chó trong lồng
    public GameObject cageClosed;     // Sprite lồng đóng
    public GameObject cageOpen;       // Sprite lồng mở
    public Transform player;          // Player reference
    public HintUI hintUI;             // Tham chiếu tới UI hiển thị Hint Text

    private bool isUnlocked = false;
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            print("ontringger");
            playerInRange = true;
            //hintUI.ShowHint("Press E to rescue");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //hintUI.HideHint();
        }
    }

    void Update()
    {
        if (playerInRange && !isUnlocked && Keyboard.current.eKey.wasPressedThisFrame)
        {
            UnlockCage();
        }
    }

    void UnlockCage()
    {
        print("Enter E");
        isUnlocked = true;
        //hintUI.HideHint();

        // 🔓 Mở lồng
        if (cageClosed != null) cageClosed.SetActive(false);
        if (cageOpen != null) cageOpen.SetActive(true);

        // 🐶 Kích hoạt animation Happy
        Animator anim = dog.GetComponent<Animator>();
        anim.SetTrigger("Rescued");

        // 🦴 Thêm script follow
        PetFollower follow = dog.AddComponent<PetFollower>();
        follow.target = player;

        // Tách chó khỏi lồng để nó có thể đi theo
        dog.transform.SetParent(null);
    //    Vector3 originalScale = dog.transform.lossyScale;
    //    dog.transform.SetParent(null);
    //    dog.transform.localScale = originalScale;
    }
}
