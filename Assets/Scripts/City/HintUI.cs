using TMPro;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    public TextMeshProUGUI hintText;

    void Start()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    public void ShowHint(string message)
    {
        if (hintText == null) return;
        hintText.text = message;
        hintText.gameObject.SetActive(true);
    }

    public void HideHint()
    {
        if (hintText == null) return;
        hintText.gameObject.SetActive(false);
    }
}
