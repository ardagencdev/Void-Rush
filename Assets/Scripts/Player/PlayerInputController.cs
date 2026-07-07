using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;

    [Header("Joystick")]
    public RectTransform joystickBG;
    public RectTransform joystickHandle;

    [Header("Joystick Settings")]
    public float joystickRange = 120f;
    [Range(0f, 0.3f)] public float deadZone = 0.08f;
    [Range(0.5f, 2f)] public float inputCurve = 0.85f;
    public float inputSmoothTime = 0.035f;
    public float handleReturnSpeed = 22f;

    private enum ControlSource
    {
        None,
        Touch,
        Mouse
    }

    private ControlSource controlSource = ControlSource.None;

    private bool isTouching;
    private int activeTouchId = -1;

    private Vector2 joystickStartPos;
    private Vector2 rawInput;
    private Vector2 smoothInput;
    private Vector2 inputSmoothVelocity;

    private void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        ResetHandleInstant();
    }

    private void Update()
    {
        if (playerMovement == null) return;

        if (Time.timeScale == 0f || playerMovement.IsGameOver)
        {
            ForceStopInput();
            return;
        }

        HandleTouchInput();
        HandleMouseInput();

        if (!isTouching)
        {
            rawInput = GetKeyboardInput();

            smoothInput = Vector2.SmoothDamp(
                smoothInput,
                rawInput,
                ref inputSmoothVelocity,
                inputSmoothTime,
                Mathf.Infinity,
                Time.unscaledDeltaTime
            );

            playerMovement.SetMoveInput(smoothInput);

            if (joystickHandle != null)
            {
                joystickHandle.anchoredPosition = Vector2.Lerp(
                    joystickHandle.anchoredPosition,
                    Vector2.zero,
                    handleReturnSpeed * Time.unscaledDeltaTime
                );
            }
        }
    }

    private Vector2 GetKeyboardInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current == null)
            return input;

        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;

        return input.sqrMagnitude > 1f ? input.normalized : input;
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;
        if (controlSource == ControlSource.Mouse) return;

        foreach (var touch in Touchscreen.current.touches)
        {
            int touchId = touch.touchId.ReadValue();
            Vector2 touchPos = touch.position.ReadValue();

            if (controlSource == ControlSource.None && touch.press.wasPressedThisFrame)
            {
                if (IsPointerInsideJoystick(touchPos))
                {
                    controlSource = ControlSource.Touch;
                    activeTouchId = touchId;
                    isTouching = true;
                    joystickStartPos = touchPos;
                }
            }

            if (controlSource == ControlSource.Touch && touchId == activeTouchId)
            {
                if (touch.press.isPressed)
                {
                    HandleJoystickInput(touchPos);
                }

                if (touch.press.wasReleasedThisFrame || !touch.press.isPressed)
                {
                    ReleaseJoystick();
                }

                return;
            }
        }
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null) return;
        if (controlSource == ControlSource.Touch) return;

        if (controlSource == ControlSource.None && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (IsPointerInsideJoystick(mousePos))
            {
                controlSource = ControlSource.Mouse;
                isTouching = true;
                joystickStartPos = mousePos;
            }
        }

        if (controlSource == ControlSource.Mouse && Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            HandleJoystickInput(mousePos);
        }

        if (controlSource == ControlSource.Mouse && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ReleaseJoystick();
        }
    }

    private void HandleJoystickInput(Vector2 currentScreenPos)
    {
        Vector2 direction = currentScreenPos - joystickStartPos;
        Vector2 normalizedInput = Vector2.ClampMagnitude(direction / joystickRange, 1f);

        rawInput = ApplyScaledRadialDeadZone(normalizedInput);

        playerMovement.SetMoveInput(rawInput);

        if (joystickHandle != null)
            joystickHandle.anchoredPosition = rawInput * joystickRange;
    }

    private Vector2 ApplyScaledRadialDeadZone(Vector2 input)
    {
        float magnitude = input.magnitude;

        if (magnitude <= deadZone)
            return Vector2.zero;

        float scaledMagnitude = Mathf.InverseLerp(deadZone, 1f, magnitude);
        scaledMagnitude = Mathf.Pow(scaledMagnitude, inputCurve);

        return input.normalized * scaledMagnitude;
    }

    private void ReleaseJoystick()
    {
        controlSource = ControlSource.None;
        activeTouchId = -1;
        isTouching = false;
        rawInput = Vector2.zero;

        smoothInput = Vector2.zero;
        inputSmoothVelocity = Vector2.zero;

        playerMovement.SetMoveInput(Vector2.zero);
    }

    private void ForceStopInput()
    {
        controlSource = ControlSource.None;
        activeTouchId = -1;
        isTouching = false;
        rawInput = Vector2.zero;
        smoothInput = Vector2.zero;
        inputSmoothVelocity = Vector2.zero;

        playerMovement.SetMoveInput(Vector2.zero);
        ResetHandleInstant();
    }

    private void ResetHandleInstant()
    {
        if (joystickHandle != null)
            joystickHandle.anchoredPosition = Vector2.zero;
    }

    private bool IsPointerInsideJoystick(Vector2 screenPos)
    {
        if (joystickBG == null) return false;

        return RectTransformUtility.RectangleContainsScreenPoint(
            joystickBG,
            screenPos,
            null
        );
    }
}