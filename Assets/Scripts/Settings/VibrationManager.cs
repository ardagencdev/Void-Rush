using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager Instance { get; private set; }

    private const string VibrationKey = "VibrationEnabled";

#if UNITY_ANDROID && !UNITY_EDITOR
    private AndroidJavaObject vibrator;
    private AndroidJavaClass vibrationEffectClass;
#endif

    public bool IsEnabled => PlayerPrefs.GetInt(VibrationKey, 1) == 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        }

        vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
#endif
    }

    public void SetVibration(bool enabled)
    {
        PlayerPrefs.SetInt(VibrationKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void VibrateLight()
    {
        Vibrate(25, 60);
    }

    public void VibrateMedium()
    {
        Vibrate(45, 120);
    }

    public void VibrateHeavy()
    {
        Vibrate(80, 200);
    }

    public void VibrateSuccess()
    {
        VibratePattern(new long[] { 0, 35, 50, 45 }, new int[] { 0, 100, 0, 160 });
    }

    public void VibrateFailure()
    {
        VibratePattern(new long[] { 0, 80, 60, 120 }, new int[] { 0, 200, 0, 255 });
    }

    private void Vibrate(long milliseconds, int amplitude)
    {
        if (!IsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator == null) return;

        int sdkInt = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");

        if (sdkInt >= 26)
        {
            AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                "createOneShot",
                milliseconds,
                Mathf.Clamp(amplitude, 1, 255)
            );

            vibrator.Call("vibrate", effect);
        }
        else
        {
            vibrator.Call("vibrate", milliseconds);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    private void VibratePattern(long[] timings, int[] amplitudes)
    {
        if (!IsEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
        if (vibrator == null) return;

        int sdkInt = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");

        if (sdkInt >= 26)
        {
            AndroidJavaObject effect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                "createWaveform",
                timings,
                amplitudes,
                -1
            );

            vibrator.Call("vibrate", effect);
        }
        else
        {
            vibrator.Call("vibrate", timings, -1);
        }
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }
}