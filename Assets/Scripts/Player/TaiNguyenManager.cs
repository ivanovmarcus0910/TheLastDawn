using TMPro;
using UnityEngine;

public class TaiNguyenManager : MonoBehaviour
{
    public TextMeshProUGUI SicText;
    public TextMeshProUGUI expText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SicText.text = 0.ToString();
        expText.text = 0.ToString();

    }
    public void UpdateUI(int sic, int exp)
    {
        SicText.text = sic.ToString();
        expText.text = exp.ToString();
    }
}
