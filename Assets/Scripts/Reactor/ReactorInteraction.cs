using UnityEngine;
public class ReactorInteraction : MonoBehaviour
{
    public CraftRecipe recipe;
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
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ReactorUIManager.Instance.ShowUI(recipe);
            ReactorUIManager.Instance.hintText.gameObject.SetActive(false);

        }
    }
}