using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private PlayerControls controls;

    public Vector2 _mouseDelta;
    public Vector2 _moveComposite;

    public float _movementInputDuration;
    public bool _movementInputDetected;

    public Action onAimActivated;
    public Action onAimDeactivated;

    public Action onCrouchActivated;
    public Action onCrouchDeactivated;

    public Action onJumpPerformed;

    public Action onLockOnToggled;

    public Action onSprintActivated;
    public Action onSprintDeactivated;

    public Action onWalkToggled;

    private void Awake()
    {
        // Initialize the generated InputActions class
        controls = new PlayerControls();

        controls.Player.Move.started += OnMove;
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;

        controls.Player.Look.performed += OnLook;

        controls.Player.Move.performed += OnMove;

        controls.Player.Jump.performed += OnJump;

        controls.Player.ToggleWalk.performed += OnToggleWalk;

        controls.Player.Sprint.started += OnSprint;
        controls.Player.Sprint.canceled += OnSprint;

        controls.Player.Crouch.started += OnCrouch;
        controls.Player.Crouch.canceled += OnCrouch;

        controls.Player.Aim.started += OnAim;
        controls.Player.Aim.canceled += OnAim;

        controls.Player.LockOn.performed += OnLockOn;
    }

    private void OnEnable()
    {
        controls.Enable(); // Enable the InputActions
    }

    private void OnDisable()
    {
        controls.Disable(); // Disable to avoid memory leaks
    }

    // --- Callback methods remain unchanged ---

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            _moveComposite = context.ReadValue<Vector2>();
            _movementInputDetected = _moveComposite.magnitude > 0;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        onJumpPerformed?.Invoke();
    }

    public void OnToggleWalk(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        onWalkToggled?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started) onSprintActivated?.Invoke();
        else if (context.canceled) onSprintDeactivated?.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started) onCrouchActivated?.Invoke();
        else if (context.canceled) onCrouchDeactivated?.Invoke();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.started) onAimActivated?.Invoke();
        if (context.canceled) onAimDeactivated?.Invoke();
    }

    public void OnLockOn(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        onLockOnToggled?.Invoke();
        onSprintDeactivated?.Invoke();
    }
}
