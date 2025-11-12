using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpaceshipEnergy : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup progressCanvasGroup;
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    [Header("Thông số")]
    public float requiredEnergy = 100f;
    public float chargeRate = 10f;

    [Header("Kích hoạt")]
    public BossGolem golemBoss;
    public float proximityRadius = 8f;

    private Transform playerTransform;
    private float currentEnergy = 0f;
    private bool isPlayerCharging = false;
    private bool isCharged = false;
    private bool hasGolemCore = false;

    [Header("Ending Cutscene")]
    public CanvasGroup fadeImage;
    public GameObject boardingPrompt;
    public Transform stairBaseLocation;
    public Collider2D mainShipCollider;
    public string playerMovementScript = "PlayerMovement"; 
    public string winSceneName = "WinScene";

    [Header("Trạng thái Tàu (GameObjects)")]
    public GameObject ship_LowEnergy;   
    public GameObject ship_FullEnergy;  
    public GameObject ship_OpenDoor;
    public TrailRenderer shipTrail;
    public float fadeDuration = 2f;
    public float takeoffSpeed = 5f;
    public float takeoffDuration = 5f;

    private bool isReadyForTakeoff = false;
    private bool isCutscenePlaying = false;

    void Start()
    {
        try
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null) throw new System.Exception("Không tìm thấy Player");
            playerTransform = playerObj.GetComponent<Transform>();
        }
        catch (Exception e)
        {
            Debug.LogError($"SpaceshipEnergy Lỗi: {e.Message}");
            this.enabled = false;
            return;
        }

        if (ship_LowEnergy != null) ship_LowEnergy.SetActive(true);
        if (ship_FullEnergy != null) ship_FullEnergy.SetActive(false);
        if (ship_OpenDoor != null) ship_OpenDoor.SetActive(false);

        if (shipTrail != null) shipTrail.emitting = false;
        if (fadeImage != null) fadeImage.alpha = 0;
        if (boardingPrompt != null) boardingPrompt.SetActive(false);

        HideUI();
        UpdateUI();
    }

    void Update()
    {
        if (isCutscenePlaying) return;

        if (playerTransform == null || !playerTransform.gameObject.activeSelf)
        {
            if (isPlayerCharging) StopCharging();
            HideUI();
            if (boardingPrompt != null) boardingPrompt.SetActive(false);
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= proximityRadius)
        {
            if (isReadyForTakeoff)
            {
                HideUI();
                if (boardingPrompt != null) boardingPrompt.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartEndingCutscene();
                }
            }
            else if (!isCharged)
            {
                ShowUI();
                StartCharging();
            }
        }
        else
        {
            HideUI();
            StopCharging();
            if (boardingPrompt != null) boardingPrompt.SetActive(false);
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
        if (progressCanvasGroup != null && progressCanvasGroup.alpha == 0)
        {
            progressCanvasGroup.alpha = 1f;
            progressCanvasGroup.interactable = true;
        }
    }

    void HideUI()
    {
        if (progressCanvasGroup != null && progressCanvasGroup.alpha == 1)
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

        if (!isReadyForTakeoff)
        {
            isReadyForTakeoff = true;
            UpdateUI();

            if (ship_LowEnergy != null) ship_LowEnergy.SetActive(false);
            if (ship_FullEnergy != null) ship_FullEnergy.SetActive(true);
        }
    }

    void UpdateUI()
    {
        float percentage = currentEnergy / requiredEnergy;
        if (progressBar != null)
            progressBar.value = percentage;

        if (progressText == null) return;

        if (isReadyForTakeoff)
        {
            progressText.text = "SẴN SÀNG CẤT CÁNH!";
        }
        else if (!hasGolemCore && currentEnergy >= (requiredEnergy * 0.9f))
        {
            progressText.text = $"90% (Cần Lõi Golem)";
        }
        else if (isCharged)
        {
            progressText.text = "HOÀN TẤT!";
        }
        else
        {
            progressText.text = $"{Mathf.FloorToInt(percentage * 100)}%";
        }
    }

    void StartEndingCutscene()
    {
        if (isCutscenePlaying) return;
        isCutscenePlaying = true;

        HideUI();
        if (boardingPrompt != null) boardingPrompt.SetActive(false);

        StartCoroutine(EndingCutsceneCoroutine());
    }

    private IEnumerator EndingCutsceneCoroutine()
    {
        if (mainShipCollider != null)
        {
            mainShipCollider.enabled = false;
        }
        if (ship_FullEnergy != null) ship_FullEnergy.SetActive(false);
        if (ship_OpenDoor != null) ship_OpenDoor.SetActive(true);

        Animator playerAnim = playerTransform.GetComponent<Animator>();
        if (playerAnim != null) playerAnim.SetBool("run", true);

        float walkSpeed = 1f; 

        while (Vector2.Distance(playerTransform.position, stairBaseLocation.position) > 0.1f)
        {
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position,
                stairBaseLocation.position,
                walkSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (playerAnim != null) playerAnim.SetBool("run", false);

        SpriteRenderer playerSprite = playerTransform.GetComponent<SpriteRenderer>();
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            Color newColor = playerSprite.color;
            newColor.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            playerSprite.color = newColor;
            yield return null;
        }

        playerTransform.gameObject.SetActive(false);

        if (ship_OpenDoor != null) ship_OpenDoor.SetActive(false);
        if (ship_FullEnergy != null) ship_FullEnergy.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(TakeoffAndWinCoroutine());
    }

    private IEnumerator TakeoffAndWinCoroutine()
    {
        if (shipTrail != null)
            shipTrail.emitting = true;

        float t = 0;
        while (t < takeoffDuration)
        {
            t += Time.deltaTime;
            transform.Translate(Vector3.up * takeoffSpeed * Time.deltaTime);

            if (fadeImage != null)
                fadeImage.alpha = Mathf.Lerp(0f, 1f, t / takeoffDuration);

            yield return null;
        }

        SceneManager.LoadScene(winSceneName);
    }
}