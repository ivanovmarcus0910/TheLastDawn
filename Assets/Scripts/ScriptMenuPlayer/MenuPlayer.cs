using UnityEngine;

public class MenuPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject panelNhiemVu;
    public GameObject panelTuiDo;
    public GameObject panelKyNang;
    public GameObject panelCaiDat;

    void Start()
    {
        ShowPanel(panelNhiemVu);
    }

    public void ShowPanel(GameObject target)
    {
        panelNhiemVu.SetActive(target == panelNhiemVu);
        panelTuiDo.SetActive(target == panelTuiDo);
        panelKyNang.SetActive(target == panelKyNang);
        panelCaiDat.SetActive(target == panelCaiDat);
    }

}
