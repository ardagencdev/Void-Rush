using UnityEngine;

public static class StatsManager
{
    private const string TotalRuns = "Stats_TotalRuns";
    private const string TotalWins = "Stats_TotalWins";
    private const string TotalDeaths = "Stats_TotalDeaths";

    private const string TotalCoins = "Stats_TotalCoins";
    private const string NormalCoins = "Stats_NormalCoins";
    private const string GoldCoins = "Stats_GoldCoins";
    private const string RareCoins = "Stats_RareCoins";

    private const string DashUses = "Stats_DashUses";
    private const string CloneUses = "Stats_CloneUses";
    private const string SlowBuffUses = "Stats_SlowBuffUses";
    private const string ArmorBuffUses = "Stats_ArmorBuffUses";
    private const string ArmorKills = "Stats_ArmorKills";

    private const string TotalPlayTime = "Stats_TotalPlayTime";

    public static void AddRun() => AddInt(TotalRuns, 1);
    public static void AddWin() => AddInt(TotalWins, 1);
    public static void AddDeath() => AddInt(TotalDeaths, 1);
    public static void AddDashUse() => AddInt(DashUses, 1);
    public static void AddCloneUse() => AddInt(CloneUses, 1);
    public static void AddSlowBuffUse() => AddInt(SlowBuffUses, 1);
    public static void AddArmorBuffUse() => AddInt(ArmorBuffUses, 1);
    public static void AddArmorKill() => AddInt(ArmorKills, 1);

    public static void AddCoin(int value, string coinType)
    {
        AddInt(TotalCoins, 1);

        if (coinType == "Normal") AddInt(NormalCoins, 1);
        else if (coinType == "Gold") AddInt(GoldCoins, 1);
        else if (coinType == "Rare") AddInt(RareCoins, 1);
    }

    public static void AddPlayTime(float seconds)
    {
        PlayerPrefs.SetFloat(TotalPlayTime, PlayerPrefs.GetFloat(TotalPlayTime, 0f) + seconds);
        PlayerPrefs.Save();
    }

    public static void SetBestTime(string key, float time)
    {
        float current = PlayerPrefs.GetFloat(key, Mathf.Infinity);

        if (time < current)
        {
            PlayerPrefs.SetFloat(key, time);
            PlayerPrefs.Save();
        }
    }

    public static int GetInt(string key) => PlayerPrefs.GetInt(key, 0);
    public static float GetFloat(string key) => PlayerPrefs.GetFloat(key, 0f);

    private static void AddInt(string key, int amount)
    {
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key, 0) + amount);
        PlayerPrefs.Save();
    }

    public static void ResetAllStats()
    {
        PlayerPrefs.DeleteKey(TotalRuns);
        PlayerPrefs.DeleteKey(TotalWins);
        PlayerPrefs.DeleteKey(TotalDeaths);

        PlayerPrefs.DeleteKey(TotalCoins);
        PlayerPrefs.DeleteKey(NormalCoins);
        PlayerPrefs.DeleteKey(GoldCoins);
        PlayerPrefs.DeleteKey(RareCoins);

        PlayerPrefs.DeleteKey(DashUses);
        PlayerPrefs.DeleteKey(CloneUses);
        PlayerPrefs.DeleteKey(SlowBuffUses);
        PlayerPrefs.DeleteKey(ArmorBuffUses);
        PlayerPrefs.DeleteKey(ArmorKills);

        PlayerPrefs.DeleteKey(TotalPlayTime);

        for (int i = 1; i <= 20; i++)
            PlayerPrefs.DeleteKey("BestTime_Level_" + i);

        PlayerPrefs.DeleteKey("BestTime_DevRoom");

        PlayerPrefs.Save();
    }
}