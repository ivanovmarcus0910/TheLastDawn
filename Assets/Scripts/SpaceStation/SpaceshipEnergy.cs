using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipEnergy : MonoBehaviour
{
    [Header("UI")]
    // Kéo "ProgressUI_Group" (có CanvasGroup) vào đây
    public CanvasGroup progressCanvasGroup;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Thông số")]
    public float requiredEnergy = 100f;
    public float chargeRate = 10f;

    [Header("Kích hoạt")]
    public BossGolem golemBoss;
    public float proximityRadius = 8f; // Khoảng cách để UI hiện lên và Golem tấn công

    private Transform playerTransform; // Để lưu vị trí Player
    private float currentEnergy = 0f;
    private bool isPlayerCharging = false;
    private bool isCharged = false;
    private bool hasGolemCore = false;

    void Start()
    {
        try
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null) throw new System.Exception("Không tìm thấy Player");

            playerTransform = playerObj.transform;
        }
        catch
        {
            this.enabled = false;
            return;
        }

        HideUI();
        UpdateUI();
    }

    void Update()
    {
        if (playerTransform == null || !playerTransform.gameObject.activeSelf)
        {
            if (isPlayerCharging)
            {
                StopCharging();
            }
            HideUI();
            return;
        }

        if (isCharged) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= proximityRadius)
        {
            ShowUI();
            StartCharging();
        }
        else
        {
            HideUI();
            StopCharging();
        }

        if (isPlayerCharging)
        {
            float maxEnergyCap = hasGolemCore ? requiredEnergy : requiredEnergy * 0.9f;
            if (currentEnergy < maxEnergyCap)
            {
                currentEnergy += chargeRate * Time.deltaTime;
                currentEnergy = Mathf.Min(currentEnergy, maxEnergyCap);
                UpdateUI();
            }

            if (currentEnergy >= requiredEnergy)
            {
                FinishCharging();
            }
        }
    }

    public void StartCharging()
    {
        if (isCharged || isPlayerCharging || !playerTransform.gameObject.activeSelf) return;

        isPlayerCharging = true;
        Debug.Log("Player trong tầm! Bắt đầu nạp...");

        if (golemBoss != null && !golemBoss.isDead && !golemBoss.isAggroed)
        {
            golemBoss.ActivateAndTeleport();
        }
    }

    public void StopCharging()
    {
        isPlayerCharging = false;
    }

    void ShowUI()
    {
        if (progressCanvasGroup.alpha == 0)
        {
            progressCanvasGroup.alpha = 1f;
            progressCanvasGroup.interactable = true;
        }
    }

    void HideUI()
    {
        if (progressCanvasGroup.alpha == 1)
        {
            progressCanvasGroup.alpha = 0f;
            progressCanvasGroup.interactable = false;
        }
    }

    public void CollectGolemCore()
    {
        hasGolemCore = true;
        Debug.Log("Đã thu thập Lõi Golem! Có thể nạp 100%");
        UpdateUI();
    }

    public bool GetIsPlayerCharging()
    {
        return isPlayerCharging;
    }

    void FinishCharging()
    {
        isCharged = true;
        isPlayerCharging = false;
        if (golemBoss != null) golemBoss.enabled = false;
        progressText.text = "HOÀN TẤT!";
        HideUI();
    }

    void UpdateUI()
    {
        float percentage = currentEnergy / requiredEnergy;
        if (progressBar != null)
            progressBar.value = percentage;

        if (progressText == null) return;

        if (isCharged)
        {
            progressText.text = "HOÀN TẤT!";
        }
        else if (!hasGolemCore && currentEnergy >= (requiredEnergy * 0.9f))
        {
            progressText.text = $"90% (Cần Lõi Golem)";
        }
        else
        {
            progressText.text = $"{Mathf.FloorToInt(percentage * 100)}%";
        }
    }
}