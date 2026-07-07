using UnityEngine;

public class ControlLayoutManager : MonoBehaviour
{
    public enum JoystickSide
    {
        Left = 0,
        Right = 1
    }

    private const string JoystickSideKey = "JoystickSide";

    public static ControlLayoutManager Instance { get; private set; }

    [Header("Default")]
    [SerializeField] private JoystickSide defaultSide = JoystickSide.Right;

    [Header("HUD References")]
    [SerializeField] private RectTransform joystick;
    [SerializeField] private RectTransform dashButton;
    [SerializeField] private RectTransform cloneButton;

    [Header("Joystick Positions")]
    [SerializeField] private Vector2 joystickLeftPos = new Vector2(170f, 170f);
    [SerializeField] private Vector2 joystickRightPos = new Vector2(-170f, 170f);

    [Header("Button Positions")]
    [SerializeField] private Vector2 dashLeftPos = new Vector2(145f, 135f);
    [SerializeField] private Vector2 dashRightPos = new Vector2(-145f, 135f);

    [SerializeField] private Vector2 cloneLeftPos = new Vector2(145f, 255f);
    [SerializeField] private Vector2 cloneRightPos = new Vector2(-145f, 255f);

    private void Awake()
    {
        Instance = this;
        ApplySavedLayout();
    }

    private void Start()
    {
        ApplySavedLayout();
    }

    public void SetJoystickLeft()
    {
        SaveAndApply(JoystickSide.Left);
    }

    public void SetJoystickRight()
    {
        SaveAndApply(JoystickSide.Right);
    }

    private void SaveAndApply(JoystickSide side)
    {
        PlayerPrefs.SetInt(JoystickSideKey, (int)side);
        PlayerPrefs.Save();

        ApplyLayout(side);
    }

    public void ApplySavedLayout()
    {
        ApplyLayout(GetSavedSide());
    }

    public JoystickSide GetSavedSide()
    {
        int defaultValue = (int)defaultSide;
        int value = PlayerPrefs.GetInt(JoystickSideKey, defaultValue);

        return value == (int)JoystickSide.Left ? JoystickSide.Left : JoystickSide.Right;
    }

    private void ApplyLayout(JoystickSide side)
    {
        bool joystickOnLeft = side == JoystickSide.Left;
        bool buttonsOnLeft = !joystickOnLeft;

        ApplyJoystick(joystickOnLeft);
        ApplyButton(dashButton, buttonsOnLeft, dashLeftPos, dashRightPos);
        ApplyButton(cloneButton, buttonsOnLeft, cloneLeftPos, cloneRightPos);
    }

    private void ApplyJoystick(bool left)
    {
        if (joystick == null) return;

        SetRect(
            joystick,
            left ? Vector2.zero : new Vector2(1f, 0f),
            left ? Vector2.zero : new Vector2(1f, 0f),
            left ? joystickLeftPos : joystickRightPos
        );
    }

    private void ApplyButton(RectTransform button, bool left, Vector2 leftPos, Vector2 rightPos)
    {
        if (button == null) return;

        SetRect(
            button,
            left ? Vector2.zero : new Vector2(1f, 0f),
            left ? Vector2.zero : new Vector2(1f, 0f),
            left ? leftPos : rightPos
        );
    }

    private void SetRect(RectTransform rect, Vector2 anchor, Vector2 pivot, Vector2 anchoredPos)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPos;
    }

    [ContextMenu("Reset Joystick Layout Save")]
    private void ResetJoystickLayoutSave()
    {
        PlayerPrefs.DeleteKey(JoystickSideKey);
        PlayerPrefs.Save();
        ApplySavedLayout();
    }
}