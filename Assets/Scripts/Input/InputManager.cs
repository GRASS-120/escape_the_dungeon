using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    private PlayerInput playerInput;

    public EventHandler OnSprint;
    public EventHandler OnSprintCanceled;
    public EventHandler OnInteract;

    void Awake() {
        // playerInput.asset.
        playerInput = new PlayerInput();
        playerInput.Player.Enable();

        playerInput.Player.Sprint.performed += OnSprint_performed;  // когда клавиша зажата или нажата
        playerInput.Player.Sprint.canceled += OnSprint_canceled;  // клавиша отжимается 
        playerInput.Player.Interact.performed += OnInteract_performed;
    }

    private void OnInteract_performed(InputAction.CallbackContext context) {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    private void OnSprint_performed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        OnSprint?.Invoke(this, EventArgs.Empty);
    }

    private void OnSprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context) {
        OnSprintCanceled?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetMovementVectorNormalized() {
        Vector3 inputVector = playerInput.Player.Movement.ReadValue<Vector3>();
        return inputVector.normalized;
    }

    // для мыши нужно создать vector2 -> mouse/delta
    // mouse/delta - изменение положения мыши между кадрами
    public Vector2 GetMouseDelta() {
        return playerInput.Player.Mouse.ReadValue<Vector2>();
    }

    // метод для получения названия клавиши (на eng раскладке) для определенного действия
    public string GetInputBindingString(string actionName) {
        InputAction action = playerInput.asset.FindAction(actionName);
        if (action != null) {
            foreach (var binding in action.bindings) {
                // Получаем человекочитаемую строку для английской раскладки
                string readableString = InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                if (!string.IsNullOrEmpty(readableString)) {
                    return readableString;
                }
            }
        }
        return "Action not found";
    }
}
