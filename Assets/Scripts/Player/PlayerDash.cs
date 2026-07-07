using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public SoundManager soundManager;

    [Header("Dash")]
    public float dashDistance = 2.5f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 2f;
    public float boundsPadding = 0.4f;

    [Header("Visual")]
    public TrailRenderer trail;

    [Header("UI")]
    public Image cooldownFill;
    public TMP_Text cooldownText;

    private bool canDash = true;
    private bool isDashing;
    private bool cooldownVisible;
    private bool gameOverHandled;

    private float cooldownTimer;
    private float textRefreshTimer;
    private const float TextRefreshInterval = 0.1f;

    public bool IsDashing => isDashing;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        SetTrail(false);
        HideCooldownUI();
    }

    private void OnEnable()
    {
        ResetDashState();
    }

    private void Update()
    {
        if (playerMovement != null && playerMovement.IsGameOver)
        {
            if (!gameOverHandled)
            {
                gameOverHandled = true;
                StopDash();
                enabled = false;
            }

            return;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            TryDash();

        if (!canDash)
            UpdateCooldownUI();
    }

    public void TryDash()
    {
        if (playerMovement != null && playerMovement.IsGameOver) return;
        if (!canDash || isDashing) return;

        if (soundManager != null)
            soundManager.PlayDashSound();

        VibrationManager.Instance?.VibrateLight();
        StatsManager.AddDashUse();

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;
        cooldownTimer = dashCooldown;
        textRefreshTimer = 0f;

        SetTrail(true);
        ShowCooldownUI();

        Vector3 startPos = transform.position;
        Vector2 dashDirection = playerMovement != null ? playerMovement.LastMoveDirection : Vector2.right;
        Vector3 endPos = ClampToBounds(startPos + (Vector3)(dashDirection * dashDistance));

        float timer = 0f;

        while (timer < dashDuration)
        {
            if (playerMovement != null && playerMovement.IsGameOver)
            {
                StopDash();
                yield break;
            }

            timer += Time.deltaTime;
            float t = dashDuration <= 0f ? 1f : timer / dashDuration;
            transform.position = ClampToBounds(Vector3.Lerp(startPos, endPos, t));

            yield return null;
        }

        transform.position = endPos;

        SetTrail(false);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        if (playerMovement != null && playerMovement.IsGameOver)
            yield break;

        canDash = true;
        cooldownTimer = 0f;
        HideCooldownUI();
    }

    private Vector3 ClampToBounds(Vector3 pos)
    {
        if (CameraWorldBounds.Instance == null) return pos;

        pos.x = Mathf.Clamp(pos.x, CameraWorldBounds.Instance.MinX + boundsPadding, CameraWorldBounds.Instance.MaxX - boundsPadding);
        pos.y = Mathf.Clamp(pos.y, CameraWorldBounds.Instance.MinY + boundsPadding, CameraWorldBounds.Instance.MaxY - boundsPadding);

        return pos;
    }

    private void UpdateCooldownUI()
    {
        if (cooldownFill == null && cooldownText == null) return;

        cooldownTimer -= Time.deltaTime;
        cooldownTimer = Mathf.Max(cooldownTimer, 0f);

        if (cooldownFill != null)
            cooldownFill.fillAmount = dashCooldown <= 0f ? 0f : cooldownTimer / dashCooldown;

        textRefreshTimer -= Time.deltaTime;

        if (textRefreshTimer <= 0f)
        {
            textRefreshTimer = TextRefreshInterval;

            if (cooldownText != null)
                cooldownText.text = cooldownTimer > 0f ? cooldownTimer.ToString("F1") : "";
        }

        if (cooldownTimer <= 0f && canDash)
            HideCooldownUI();
    }

    private void ShowCooldownUI()
    {
        if (cooldownVisible) return;

        cooldownVisible = true;

        if (cooldownFill != null)
            cooldownFill.gameObject.SetActive(true);
    }

    private void SetTrail(bool state)
    {
        if (trail != null)
            trail.emitting = state;
    }

    private void HideCooldownUI()
    {
        cooldownVisible = false;

        if (cooldownFill != null)
        {
            cooldownFill.gameObject.SetActive(false);
            cooldownFill.fillAmount = 0f;
        }

        if (cooldownText != null)
            cooldownText.text = "";
    }

    public void ResetDashState()
    {
        StopAllCoroutines();

        canDash = true;
        isDashing = false;
        gameOverHandled = false;
        cooldownTimer = 0f;
        textRefreshTimer = 0f;

        SetTrail(false);
        HideCooldownUI();
    }

    public void StopDash()
    {
        StopAllCoroutines();

        isDashing = false;
        canDash = false;
        cooldownTimer = 0f;

        SetTrail(false);
        HideCooldownUI();
    }
}