using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour {
    [Header("Entities")]
    [SerializeField] private Player player;
    [SerializeField] private InputManager inputManager;

    [Header("UI")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;

    private InteractiveObject selectedObject;
    private InteractiveObjectSO selectedObjectSO;
    private Vector3 defaultScale;
    private Vector3 selectedScale;

    private void Awake() {
        defaultScale = Vector3.one;
        selectedScale = new Vector3(defaultScale.x, defaultScale.y, 0) * 1.5f;
    }

    // ! мне кажется это можно как-то оптимизировать - то есть не вызывать каждый кадр Change если HasSelectedObject
    // ! а как-то по-другому...
    private void Update() {
        if (player.HasSelectedObject()) {
            selectedObject = player.GetSelectedObject();
            selectedObjectSO = selectedObject.GetObjectSO();

            string interactKeyName = inputManager.GetInputBindingString("Interact");
            string newText = "";

            // ? state machine
            if (selectedObjectSO is ClosableObjectSO closableObjectSO) {
                newText = $"Использовать {closableObjectSO.objectName} [{interactKeyName}]"; 
            } else if (selectedObjectSO is ItemObjectSO itemObjectSO) {
                newText = $"Взять {itemObjectSO.objectName} [{interactKeyName}]"; 
            }

            ChangeImageColor(selectedColor);
            ChangeImageScale(selectedScale);
            ChangeTextFieldValue(newText);
        } else {
            ChangeImageColor(defaultColor);
            ChangeImageScale(defaultScale);
            ChangeTextFieldValue("");
        }     
    }

    private void ChangeImageColor(Color newColor) {
        image.color = newColor;
    }

    private void ChangeImageScale(Vector3 newScale) {
        image.transform.localScale = newScale;
    }

    private void ChangeTextFieldValue(string newText) {
        textField.text = newText;
    }
}
