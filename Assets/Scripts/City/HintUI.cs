using TMPro;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI hintText;

    void Start()
    {
        HideHint();
    }

    public void ShowHint(string message)
    {
        if (hintText != null)
        {
            hintText.gameObject.SetActive(true);
            hintText.text = message;
        }
    }

    public void HideHint()
    {
        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
    }
}
