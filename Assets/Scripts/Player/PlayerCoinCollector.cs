using UnityEngine;
using TMPro;

public class PlayerCoinCollector : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public EnemySpawner enemySpawner;
    public SoundManager soundManager;
    public ScoreUIEffect scoreUIEffect;
    public ComboUI comboUI;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("Score")]
    public int winScore = 15;

    [Header("Combo")]
    public bool comboEnabled = true;
    public float comboTimeLimit = 1.5f;
    public int maxCombo = 3;
    public int coinsForCombo2 = 2;
    public int coinsForCombo3 = 5;

    [Tooltip("Level configden gelir. Boşsa eski coinsForCombo2/3 sistemi kullanılır.")]
    public ComboSpeedStage[] comboSpeedStages;

    private int score = 0;
    private int combo = 1;
    private int comboChain = 0;
    private float comboTimer = 0f;

    public GameStateManager gameStateManager;

    public int Score => score;
    public int Combo => combo;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        UpdateScoreUI();

        if (comboUI != null)
            comboUI.UpdateCombo(1);
    }

    private void Update()
    {
        if (playerMovement != null && playerMovement.IsGameOver) return;
        if (!comboEnabled) return;
        if (comboChain <= 0) return;

        comboTimer += Time.deltaTime;

        float normalizedTime = 1f - (comboTimer / comboTimeLimit);

        if (comboUI != null)
            comboUI.UpdateTimerBar(normalizedTime, combo);

        if (comboTimer > comboTimeLimit)
            ResetCombo();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerMovement != null && playerMovement.IsGameOver) return;
        if (!other.CompareTag("Coin")) return;

        CollectCoin(other);
    }

    private void CollectCoin(Collider2D coinCollider)
    {
        coinCollider.enabled = false;

        VibrationManager.Instance?.VibrateLight();

        Coin coin = coinCollider.GetComponent<Coin>();

        int currentCombo = 1;

        if (comboEnabled)
        {
            comboTimer = 0f;
            comboChain++;
            combo = GetComboFromChain();
            currentCombo = combo;
        }
        else
        {
            combo = 1;
            comboChain = 0;
            comboTimer = 0f;
        }

        int gainedScore = 1;

        if (coin != null)
            gainedScore = coin.value * currentCombo;

        score += gainedScore;

        if (coin != null)
        {
            string coinType = "Normal";

            if (coin.value == 5)
                coinType = "Gold";
            else if (coin.value == 10)
                coinType = "Rare";

            StatsManager.AddCoin(gainedScore, coinType);
        }

        UpdateScoreUI();

        if (enemySpawner != null)
            enemySpawner.TrySpawnBoss(score);

        if (comboUI != null && comboEnabled)
            comboUI.ShowCombo(gainedScore, currentCombo);

        if (scoreUIEffect != null)
            scoreUIEffect.PlayPop();

        SpawnScaleEffect coinEffect = coinCollider.GetComponentInChildren<SpawnScaleEffect>();

        if (coinEffect != null)
            coinEffect.Collect();
        else
            Destroy(coinCollider.gameObject);

        if (soundManager != null)
            soundManager.PlayCoinSound();

        if (score >= winScore && gameStateManager != null)
            gameStateManager.WinGame(score);
    }

    private int GetComboFromChain()
    {
        if (comboSpeedStages != null && comboSpeedStages.Length > 0)
        {
            int result = 1;

            for (int i = 0; i < comboSpeedStages.Length; i++)
            {
                ComboSpeedStage stage = comboSpeedStages[i];

                if (stage == null) continue;
                if (stage.comboMultiplier < 2) continue;
                if (stage.coinsRequired < 1) continue;

                if (comboChain >= stage.coinsRequired)
                    result = Mathf.Max(result, stage.comboMultiplier);
            }

            return Mathf.Clamp(result, 1, maxCombo);
        }

        int fallbackResult = 1;

        if (comboChain >= coinsForCombo3)
            fallbackResult = 3;
        else if (comboChain >= coinsForCombo2)
            fallbackResult = 2;

        return Mathf.Clamp(fallbackResult, 1, maxCombo);
    }

    private void ResetCombo()
    {
        combo = 1;
        comboChain = 0;
        comboTimer = 0f;

        if (comboUI != null)
        {
            comboUI.ResetCombo();
            comboUI.UpdateTimerBar(0f, combo);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}