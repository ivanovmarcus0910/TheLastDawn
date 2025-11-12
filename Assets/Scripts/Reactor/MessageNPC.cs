using UnityEngine;
using TMPro;
using System.Collections;

public class MessageNPC : MonoBehaviour
{
    public static MessageNPC Instance;

    [Header("References")]
    public RectTransform npcTransform;
    public TextMeshProUGUI messageText;
    public GameObject messagePanel;
    public Animator birdAnimator;

    [Header("Settings")]
    public float moveSpeed = 500f;    
    public float textSpeed = 0.05f;     
    public float messageHoldTime = 1f;  

    private Vector2 startPos;
    private Vector2 middlePos;
    private bool isShowing = false;
    private string currentMessage;

    void Awake()
    {
        Instance = this;

        float yPos = Screen.height / 4f;
        startPos = new Vector2(-Screen.width / 2 - 500, yPos);
        middlePos = new Vector2(-Screen.width / 3, yPos);

        npcTransform.anchoredPosition = startPos;
        messagePanel.SetActive(false);
    }

    public void Show(string message)
    {
        if (isShowing) return;

        currentMessage = message;
        messageText.text = "";
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        isShowing = true;
        birdAnimator.Play("Fly");
        while (Vector2.Distance(npcTransform.anchoredPosition, middlePos) > 10f)
        {
            npcTransform.anchoredPosition = Vector2.MoveTowards(
                npcTransform.anchoredPosition, middlePos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        messagePanel.SetActive(true);
        yield return StartCoroutine(TypeText(currentMessage));

        yield return new WaitForSeconds(messageHoldTime);
        messagePanel.SetActive(false);

        npcTransform.localScale = new Vector3(-npcTransform.localScale.x, npcTransform.localScale.y, npcTransform.localScale.z);

        while (Vector2.Distance(npcTransform.anchoredPosition, startPos) > 10f)
        {
            npcTransform.anchoredPosition = Vector2.MoveTowards(
                npcTransform.anchoredPosition, startPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        npcTransform.localScale = new Vector3(-npcTransform.localScale.x, npcTransform.localScale.y, npcTransform.localScale.z);

        isShowing = false;
    }

    private IEnumerator TypeText(string text)
    {
        messageText.text = "";
        foreach (char c in text)
        {
            messageText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
