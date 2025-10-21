using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Buttons")]
    public Button btnNhiemVu;
    public Button btnTuiDo;
    public Button btnKiNang;
    public Button btnCaiDat;
    public Button btnClose;


    [Header("Panels (CanvasGroup)")]
    public CanvasGroup panelNhiemVu;
    public CanvasGroup panelTuiDo;
    public CanvasGroup panelKiNang;
    public CanvasGroup panelCaiDat;

    [Header("Menu CanvasGroup")]
    public CanvasGroup mainMenuGroup;
    private void Start()
    {
        // Gắn sự kiện bấm nút
        btnNhiemVu.onClick.AddListener(() => ShowPanel(panelNhiemVu));
        btnTuiDo.onClick.AddListener(() => ShowPanel(panelTuiDo));
        btnKiNang.onClick.AddListener(() => ShowPanel(panelKiNang));
        btnCaiDat.onClick.AddListener(() => ShowPanel(panelCaiDat));
        btnClose.onClick.AddListener(HideAll);

        // Ẩn hết panel lúc đầu trừ túi đồ (hoặc panel mặc định)
        ShowPanel(panelTuiDo);
    }

    private void ShowPanel(CanvasGroup activePanel)
    {
        CanvasGroup[] panels = { panelNhiemVu, panelTuiDo, panelKiNang, panelCaiDat };

        foreach (var panel in panels)
        {
            bool isActive = (panel == activePanel);
            panel.alpha = isActive ? 1 : 0;
            panel.interactable = isActive;
            panel.blocksRaycasts = isActive;
        }
    }
    public void HideAll()
    {
        // Ẩn toàn bộ panel
        StartCoroutine(Fade(mainMenuGroup, 0));
        mainMenuGroup.interactable = false;
        mainMenuGroup.blocksRaycasts = false;
    }
    private IEnumerator Fade(CanvasGroup group, float targetAlpha, float duration = 0.2f)
    {
        float start = group.alpha;
        float time = 0f;
        while (time < duration)
        {
            group.alpha = Mathf.Lerp(start, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        group.alpha = targetAlpha;
    }
}
