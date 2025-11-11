using UnityEngine;
using UnityEngine.InputSystem;
public class ReactorInteraction : MonoBehaviour
{
    private bool playerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ReactorUIManager.Instance.hintText.gameObject.SetActive(true);

            Debug.Log("Press E to open Reactor UI");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ReactorUIManager.Instance.hintText.gameObject.SetActive(false);

            ReactorUIManager.Instance.HideUI();
        }
    }

    void Update()
    {
        
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("gọi hàm show");
            ReactorUIManager.Instance.ShowRecipeList();
            ReactorUIManager.Instance.hintText.gameObject.SetActive(false);
        }
    }
}