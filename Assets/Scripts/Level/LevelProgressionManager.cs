using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressionManager : MonoBehaviour
{
    public static LevelProgressionManager Instance;

    [Header("Levels")]
    public LevelConfig[] levels;

    [Header("Scene")]
    public string gameSceneName = "a";

    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLevelMode()
    {
        SelectedLevelData.SetMission(levels[0]);
        currentLevelIndex = 0;
        LoadCurrentLevel();
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            Debug.Log("All levels completed!");
            SceneManager.LoadScene("MainMenu");
            return;
        }

        LoadCurrentLevel();
    }

    private void LoadCurrentLevel()
    {
        SelectedLevelData.selectedLevel = levels[currentLevelIndex];
        SceneManager.LoadScene(gameSceneName);
    }
}