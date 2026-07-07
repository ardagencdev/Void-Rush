using TMPro;
using UnityEngine;

public class BestTimeDisplay : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI bestTimeText;

    [Header("Text")]
    public string label = "Best Record";

    private void Start()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (bestTimeText == null)
            return;

        string bestTimeKey = GetBestTimeKey();
        float bestTime = PlayerPrefs.GetFloat(bestTimeKey, -1f);

        if (bestTime < 0f)
        {
            bestTimeText.text = $"{label}:\n--";
            return;
        }

        bestTimeText.text = $"{label}:\n{bestTime:F1}s";
    }

    private string GetBestTimeKey()
    {
        if (SelectedLevelData.isLevelMode && SelectedLevelData.selectedLevel != null)
            return "BestTime_Level_" + SelectedLevelData.selectedLevel.levelNumber;

        return "BestTime_DevRoom";
    }
}