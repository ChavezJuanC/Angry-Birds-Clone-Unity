using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{

    public static PlayerInput PlayerInput;

    private InputAction _mousePositionAction;
    private InputAction _mouseAction;

    public static Vector2 MousePositon;

    public static bool WasLeftMouseButtonPresses;
    public static bool WasLeftMouseButtonReleased;
    public static bool IsLeftMouseButtonPressed;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _mousePositionAction = PlayerInput.actions["MousePosition"];
        _mouseAction = PlayerInput.actions["Mouse"];
    }

    private void Update()
    {
        MousePositon = _mousePositionAction.ReadValue<Vector2>();

        WasLeftMouseButtonPresses = _mouseAction.WasPerformedThisFrame();
        WasLeftMouseButtonReleased = _mouseAction.WasReleasedThisFrame();
        IsLeftMouseButtonPressed = _mouseAction.IsPressed();
    }
}
