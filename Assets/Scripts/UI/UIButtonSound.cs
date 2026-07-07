using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    private Button button;
    private SoundManager soundManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        soundManager = FindFirstObjectByType<SoundManager>();

        if (button != null)
            button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (soundManager != null)
            soundManager.PlayButtonClickSound();

        VibrationManager.Instance?.VibrateLight();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(PlayClickSound);
    }
}