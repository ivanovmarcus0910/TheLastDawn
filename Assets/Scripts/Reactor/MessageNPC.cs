using UnityEngine;
using TMPro;
using System.Collections;

public class MessageNPC : MonoBehaviour
{
    public static MessageNPC Instance;

    [Header("References")]
    public RectTransform npcTransform; // Chính đối tượng MessageNPC
    public TextMeshProUGUI messageText;
    public GameObject messagePanel;
    public Animator birdAnimator;

    [Header("Settings")]
    public float moveSpeed = 500f;      // Tốc độ bay
    public float textSpeed = 0.05f;     // Tốc độ hiện từng ký tự
    public float messageHoldTime = 1f;  // Thời gian giữ chữ sau khi hiện xong

    private Vector2 startPos;
    private Vector2 middlePos;
    private bool isShowing = false;
    private string currentMessage;

    void Awake()
    {
        Instance = this;

        float yPos = Screen.height / 4f; // Vị trí dọc
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

        // Bay vào giữa
        while (Vector2.Distance(npcTransform.anchoredPosition, middlePos) > 10f)
        {
            npcTransform.anchoredPosition = Vector2.MoveTowards(
                npcTransform.anchoredPosition, middlePos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Hiển thị text
        messagePanel.SetActive(true);
        yield return StartCoroutine(TypeText(currentMessage));

        // Giữ nguyên chữ
        yield return new WaitForSeconds(messageHoldTime);
        messagePanel.SetActive(false);

        // Đảo hướng ngang bằng nhân -1 cho localScale.x
        npcTransform.localScale = new Vector3(-npcTransform.localScale.x, npcTransform.localScale.y, npcTransform.localScale.z);

        // Bay lui về startPos
        while (Vector2.Distance(npcTransform.anchoredPosition, startPos) > 10f)
        {
            npcTransform.anchoredPosition = Vector2.MoveTowards(
                npcTransform.anchoredPosition, startPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Trả lại hướng ban đầu
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
