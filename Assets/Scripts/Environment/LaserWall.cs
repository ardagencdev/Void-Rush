using UnityEngine;

public class LaserWall : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifeTime = 1.5f;

    [Header("Sound")]
    public AudioClip laserLoopSound;
    public float volume = 1f;

    private AudioSource audioSource;

    private void Start()
    {
        SetupAudio();
        Destroy(gameObject, lifeTime);
    }

    private void SetupAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = laserLoopSound;
        audioSource.volume = volume * SoundManager.SFXVolume;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;

        if (laserLoopSound != null)
            audioSource.Play();
    }

    public void FreezeLaser()
    {
        StopAllCoroutines();

        if (audioSource != null)
            audioSource.Stop();

        enabled = false;
    }

    public void PauseLaserSound()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }

    public void ResumeLaserSound()
    {
        if (audioSource != null)
            audioSource.UnPause();
    }
}
