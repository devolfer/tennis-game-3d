using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputEventRelay", menuName = "Scriptable Object/Event Relay/Input")]
public class InputEventRelay : ScriptableObject, Controls.IPlayerActions {
    public event Action<Vector2> OnMoveEvent;
    public event Action<Vector2, bool> OnAimEvent;
    public event Action OnShotFlatEvent;
    public event Action OnShotFlatCancelledEvent;
    public event Action OnShotTopSpinEvent;
    public event Action OnShotTopSpinCancelledEvent;
    public event Action OnShotSliceEvent;
    public event Action OnShotSliceCancelledEvent;
    public event Action OnShotDropOrLobEvent;
    public event Action OnShotDropOrLobCancelledEvent;

    public event Action OnPauseEvent;

    private Controls controls;

    private void OnEnable() {
        if (controls == null) {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }

        Enable();
    }

    private void OnDisable() {
        Disable();
    }

    public void Enable() {
        controls.Player.Enable();
    }

    public void Disable() {
        controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context) {
        OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnAim(InputAction.CallbackContext context) {
        bool isMouse = context.control.device.name == "Mouse";
        OnAimEvent?.Invoke(context.ReadValue<Vector2>(), isMouse);
    }

    public void OnShot_Flat(InputAction.CallbackContext context) {
        if (context.performed) {
            OnShotFlatEvent?.Invoke();
        }

        if (context.canceled) {
            OnShotFlatCancelledEvent?.Invoke();
        }
    }

    public void OnShot_TopSpin(InputAction.CallbackContext context) {
        if (context.performed) {
            OnShotTopSpinEvent?.Invoke();
        }

        if (context.canceled) {
            OnShotTopSpinCancelledEvent?.Invoke();
        }
    }

    public void OnShot_Slice(InputAction.CallbackContext context) {
        if (context.performed) {
            OnShotSliceEvent?.Invoke();
        }

        if (context.canceled) {
            OnShotSliceCancelledEvent?.Invoke();
        }
    }

    public void OnShot_Drop_or_Lob(InputAction.CallbackContext context) {
        if (context.performed) {
            OnShotDropOrLobEvent?.Invoke();
        }

        if (context.canceled) {
            OnShotDropOrLobCancelledEvent?.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context) {
        if (!context.performed) return;
        
        OnPauseEvent?.Invoke();
    }
}