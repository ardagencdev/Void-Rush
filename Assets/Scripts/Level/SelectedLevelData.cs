public enum GameLaunchMode
{
    DevRoom,
    Mission
}

public static class SelectedLevelData
{
    public static LevelConfig selectedLevel;
    public static GameLaunchMode launchMode;

    public static bool isLevelMode => launchMode == GameLaunchMode.Mission;

    public static void SetDevRoom(LevelConfig config)
    {
        launchMode = GameLaunchMode.DevRoom;
        selectedLevel = config;
    }

    public static void SetMission(LevelConfig config)
    {
        launchMode = GameLaunchMode.Mission;
        selectedLevel = config;
    }

    public static void Clear()
    {
        selectedLevel = null;
        launchMode = GameLaunchMode.DevRoom;
    }
}