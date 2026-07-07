using UnityEngine;

public class AudioSettingsApply : MonoBehaviour
{
    public enum SoundType
    {
        Music,
        SFX
    }

    [Header("Settings")]
    [SerializeField] private SoundType soundType;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Apply();
    }

    public void Apply()
    {
        if (audioSource == null)
        {
            Debug.LogWarning($"{gameObject.name} üzerinde AudioSource yok.");
            return;
        }

        bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;

        float volume = soundType == SoundType.Music
            ? PlayerPrefs.GetFloat("MusicVolume", 1f)
            : PlayerPrefs.GetFloat("SFXVolume", 1f);

        audioSource.volume = soundOn ? volume : 0f;
    }
}