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
    public string playerMovementScript = "PlayerMovement"; // Đảm bảo gõ đúng tên script
    public string winSceneName = "WinScene";

    [Header("Trạng thái Tàu (GameObjects)")]
    public GameObject ship_LowEnergy;   // Kéo GameObject Ship_LowEnergy (con) vào đây
    public GameObject ship_FullEnergy;  // Kéo GameObject Ship_FullEnergy (con) vào đây
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
            playerTransform = playerObj.transform;
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
        // 1. Nếu cutscene đang chạy, không làm gì cả
        if (isCutscenePlaying) return;

        // 2. Kiểm tra Player (chết hoặc không tồn tại)
        if (playerTransform == null || !playerTransform.gameObject.activeSelf)
        {
            if (isPlayerCharging) StopCharging();
            HideUI();
            if (boardingPrompt != null) boardingPrompt.SetActive(false); // Sửa lỗi: Ẩn prompt khi chết
            return;
        }

        // --- Player còn sống và không có cutscene ---

        // 3. Kiểm tra khoảng cách
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= proximityRadius)
        {
            // Player đang ở GẦN tàu
            if (isReadyForTakeoff)
            {
                // Đã nạp xong, sẵn sàng bay
                HideUI(); // Ẩn thanh nạp
                if (boardingPrompt != null) boardingPrompt.SetActive(true); // Hiện "Nhấn [E]"

                if (Input.GetKeyDown(KeyCode.E))
                {
                    StartEndingCutscene();
                }
            }
            else if (!isCharged)
            {
                // Chưa nạp xong, bắt đầu nạp
                ShowUI();
                StartCharging();
            }
        }
        else
        {
            // Player ở XA tàu
            HideUI();
            StopCharging();
            if (boardingPrompt != null) boardingPrompt.SetActive(false); // Ẩn "Nhấn [E]"
        }

        // 4. Logic Nạp năng lượng (chỉ chạy nếu được kích hoạt)
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

        // Sửa lỗi: Thay đổi thứ tự logic
        if (isReadyForTakeoff)
        {
            progressText.text = "SẴN SÀNG CẤT CÁNH!";
        }
        else if (!hasGolemCore && currentEnergy >= (requiredEnergy * 0.9f))
        {
            progressText.text = $"90% (Cần Lõi Golem)";
        }
        else if (isCharged) // Chỉ là dự phòng
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

        (playerTransform.GetComponent(playerMovementScript) as MonoBehaviour).enabled = false;

        StartCoroutine(EndingCutsceneCoroutine());
    }

    private IEnumerator EndingCutsceneCoroutine()
    {
        if (mainShipCollider != null)
        {
            mainShipCollider.enabled = false;
        }
        // BƯỚC 2: Mở cửa và hiện cầu thang
        if (ship_FullEnergy != null) ship_FullEnergy.SetActive(false);
        if (ship_OpenDoor != null) ship_OpenDoor.SetActive(true);

        Animator playerAnim = playerTransform.GetComponent<Animator>();
        if (playerAnim != null) playerAnim.SetBool("run", true); // Bật anim đi bộ

        float walkSpeed = 1f; // Tốc độ Player đi

        // Di chuyển Player đến vị trí "StairBaseLocation"
        while (Vector2.Distance(playerTransform.position, stairBaseLocation.position) > 0.1f)
        {
            playerTransform.position = Vector3.MoveTowards(
                playerTransform.position,
                stairBaseLocation.position, // Sử dụng vị trí chân cầu thang
                walkSpeed * Time.deltaTime
            );
            yield return null; // Chờ 1 frame
        }

        if (playerAnim != null) playerAnim.SetBool("run", false);

        // BƯỚC 3: Làm mờ Player
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

        // BƯỚC 4 (Phần 1): Đóng cửa và ẩn cầu thang
        if (ship_OpenDoor != null) ship_OpenDoor.SetActive(false); // Cầu thang tự ẩn theo
        if (ship_FullEnergy != null) ship_FullEnergy.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // BƯỚC 4 & 5 (Phần 2): Bay lên
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

        // BƯỚC 5: Tải Scene "YOU WIN"
        SceneManager.LoadScene(winSceneName);
    }
}