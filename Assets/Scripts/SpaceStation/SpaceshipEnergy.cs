using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipEnergy : MonoBehaviour
{
    [Header("UI")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Thông số")]
    public float requiredEnergy = 100f;
    public float chargeRate = 10f;

    public BossGolem golemBoss;

    private float currentEnergy = 0f;
    private bool isPlayerCharging = false;
    private bool isCharged = false;

    private bool hasGolemCore = false;

    void Start()
    {
        UpdateUI();
    }
    void Update()
    {
        if (isCharged || !isPlayerCharging) return;

        float maxEnergyCap = hasGolemCore ? requiredEnergy : requiredEnergy * 0.9f;

        if (currentEnergy < maxEnergyCap)
        {
            currentEnergy += chargeRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergyCap); 
        }

        UpdateUI();

        if (currentEnergy >= requiredEnergy)
        {
            FinishCharging();
        }
    }

    public void StartCharging()
    {
        if (isCharged) return;
        isPlayerCharging = true;

        if (golemBoss != null && !golemBoss.isDead)
        {
            golemBoss.ActivateAndTeleport();
        }
    }

    public void StopCharging()
    {
        isPlayerCharging = false;
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
        progressText.text = "NẠP NĂNG LƯỢNG HOÀN THÀNH!";
    }

    void UpdateUI()
    {
        float percentage = currentEnergy / requiredEnergy;
        progressBar.value = percentage;

        if (isCharged)
        {
            progressText.text = "NẠP NĂNG LƯỢNG HOÀN THÀNH!";
        }
        else if (!hasGolemCore && currentEnergy >= (requiredEnergy * 0.9f))
        {
            progressText.text = $"TỐI ĐA 90% (Cần Lõi Golem)";
        }
        else
        {
            progressText.text = $"NĂNG LƯỢNG: {Mathf.FloorToInt(percentage * 100)}%";
        }
    }
}
