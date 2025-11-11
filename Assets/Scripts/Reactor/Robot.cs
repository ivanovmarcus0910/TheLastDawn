using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; // Đã có
using UnityEngine.UI;

public class Robot : MonoBehaviour
{
    // Các biến Public
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    // Các biến Private
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    // Biến mới để theo dõi trạng thái Player trong phạm vi
    private bool isPlayerInRange = false;

    // ==========================================================
    // LOGIC PHẠM VI (TRIGGER)
    // ==========================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // ==========================================================
    // LOGIC INPUT (Kiểm tra nhấn phím E)
    // ==========================================================
    private void Update()
    {
        // Chỉ kiểm tra phím E nếu Player đang trong phạm vi
        if (isPlayerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (dialogueData == null)
                return;

            if (isDialogueActive)
            {
                // Hội thoại đang mở -> Chuyển dòng hoặc Skip Typing
                NextLine();
            }
            else
            {
                // Hội thoại chưa mở -> Bắt đầu hội thoại
                StartDialogue();
            }
        }
    }

    // ==========================================================
    // LOGIC HỘI THOẠI (Giữ nguyên)
    // ==========================================================

    private void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
    }

    private void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
    }
}