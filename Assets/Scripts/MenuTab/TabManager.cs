using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
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

    private bool isMenuOpen = false;
    private bool isFading = false;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        if (btnNhiemVu != null) btnNhiemVu.onClick.AddListener(() => ShowPanel(panelNhiemVu));
        if (btnTuiDo != null) btnTuiDo.onClick.AddListener(() => ShowPanel(panelTuiDo));
        if (btnKiNang != null) btnKiNang.onClick.AddListener(() => ShowPanel(panelKiNang));
        if (btnCaiDat != null) btnCaiDat.onClick.AddListener(() => ShowPanel(panelCaiDat));
        if (btnClose != null) btnClose.onClick.AddListener(() => SetMenuState(false));

        SetMenuState(false, true);  // Đóng ngay đầu game
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (isFading) return;   // tránh spam fade
        SetMenuState(!isMenuOpen);
    }

    public void SetMenuState(bool open, bool skipFade = false)
    {
        // Cập nhật ngay lập tức trạng thái menu (để Toggle không bị sai)
        isMenuOpen = open;

        if (mainMenuGroup != null)
        {
            mainMenuGroup.interactable = open;
            mainMenuGroup.blocksRaycasts = open;

            float targetAlpha = open ? 1 : 0;

            if (skipFade)
            {
                if (fadeRoutine != null) StopCoroutine(fadeRoutine);
                mainMenuGroup.alpha = targetAlpha;
            }
            else
            {
                if (fadeRoutine != null) StopCoroutine(fadeRoutine);
                fadeRoutine = StartCoroutine(Fade(mainMenuGroup, targetAlpha));
            }
        }

        // hiển thị panel mặc định khi mở menu
        if (open)
            ShowPanel(panelTuiDo);
        else
            ShowPanel(null);
    }

    private void ShowPanel(CanvasGroup activePanel)
    {
        CanvasGroup[] panels = { panelNhiemVu, panelTuiDo, panelKiNang, panelCaiDat };

        foreach (var panel in panels)
        {
            if (panel == null) continue;

            bool show = (panel == activePanel);
            panel.alpha = show ? 1f : 0f;
            panel.interactable = show;
            panel.blocksRaycasts = show;
        }
    }

    private IEnumerator Fade(CanvasGroup group, float targetAlpha, float duration = 0.2f)
    {
        isFading = true;

        float start = group.alpha;
        float t = 0f;

        while (t < duration)
        {
            group.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        group.alpha = targetAlpha;
        isFading = false;
    }
}
