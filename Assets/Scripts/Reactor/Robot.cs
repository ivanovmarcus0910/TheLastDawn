using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Robot : MonoBehaviour
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage; // ?ã thêm d?u ch?m ph?y b? thi?u

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;


    private void OnTriggerStay2D(Collider2D other)
    {
        // 1. Ki?m tra xem ??i t??ng va ch?m có ph?i là Player không
        if (other.CompareTag("Player"))
        {
            // 2. Ki?m tra xem ng??i ch?i có nh?n phím T??ng tác (E) không
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (dialogueData == null)
                    return;

                if (isDialogueActive)
                {
                    // H?i tho?i ?ang ho?t ??ng: Chuy?n dòng ho?c Skip Typing
                    NextLine();
                }
                else
                {
                    // H?i tho?i ch?a ho?t ??ng: B?t ??u h?i tho?i
                    StartDialogue();
                }
            }
        }
    }

        void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        TypeLine();
    }

    void NextLine()
    {
        if (isTyping)
        {
            // Skip typing animation and show the full line
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            // If another line, type next line
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
        // D?ng m?i hi?u ?ng gõ ch? ?ang ch?y
        StopAllCoroutines();

        // ??t tr?ng thái h?i tho?i v? false
        isDialogueActive = false;

        // Xóa n?i dung hi?n th?
        dialogueText.SetText("");

        // ?n panel h?i tho?i
        dialoguePanel.SetActive(false);

    }
}
