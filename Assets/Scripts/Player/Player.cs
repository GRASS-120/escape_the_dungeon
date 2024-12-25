using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// ! ограничение на длительность звука + нельзя кучу раз начинать проигрывать анимацию - ограничение

// ! что делать
// 1. доделать указатель (что подсвечивался и тп)
// 2. держать предметы в руке -> куда викидывать предметы? не понятно, поэтмоу думаю нужен полноценный инвентарь...
// 3. инвентарь

// ! нужна утилита для дебага (например через DrawRay), есть видос

// ! это нужно исправить в след версии контроллера
// 1. исправить звук шагов по лестнице (сделать слой лестницы и проверять его, если лестница => менять интервал?)
// 2. придумать, как решить проблему со звуком приземления после прыжка (+ просто приземления после падения)
// 3. при ЛЮБОЙ (переход по ступенькам, плавный спуск с поверхности, спуск с поверхности с минимальной разницей
// в высоте) смене высот isGrounded становиться false. Как это фиксить?

public class Player : MonoBehaviour {
    [Header("Entities")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerAudio playerAudio;
    [SerializeField] private LayerMask interactiveObjectLayer;

    [Header("Movement params")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump params")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Mouse sensitivity params")]
    [SerializeField] private float mouseSensitivity = 4f;
    [SerializeField] private float upDownRange = 80f;

    [Header("Footsteps params")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float velocityThreshold = 2f;

    // ивенты часто нужно для того, чтобы уведомлять о смене состояния скрипт для анимаций и тп
    public event EventHandler<OnSelectedObjectChangedArgs> OnSelectedObjectChanged;
    public class OnSelectedObjectChangedArgs : EventArgs {
        public InteractiveObject selectedObject;
    }

    private Vector3 moveDir = Vector3.zero;
    private InteractiveObject selectedObject;  // для хранения объекта, с которым можно по-взаимодействовать

    private bool isSprinting = false;
    private float nextStepTime;
    private float verticalRotation;
    private bool canPlayFallSound = true;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputManager.OnInteract += InputManager_OnInteract;
        inputManager.OnSprint += InputManager_OnSprint;
        inputManager.OnSprintCanceled += InputManager_OnSprintCanceled;
    }

    private void InputManager_OnInteract(object sender, EventArgs e) {
        if (selectedObject != null) {
            selectedObject.Interact(this);
        }
    }

    private void InputManager_OnSprint(object sender, System.EventArgs e) {
        isSprinting = true;
    }

    private void InputManager_OnSprintCanceled(object sender, System.EventArgs e) {
        isSprinting = false;
    }

    private void Update() {
        HandleMovement();
        HandleRotation();
        HandleInteraction();
        HandleFootsteps();
    }

    private void HandleMovement() {
        Vector3 inputDir = inputManager.GetMovementVectorNormalized();
        // либо shift нажат и множитель есть, либо он не нажат и обычная ходьба
        float speedMultiplier = isSprinting && characterController.isGrounded ? sprintMultiplier : 1f;  

        Vector3 horizontalMoveDir = new Vector3(inputDir.x, 0, inputDir.z) * moveSpeed * speedMultiplier;
        horizontalMoveDir = transform.rotation * horizontalMoveDir;  // ?

        moveDir.x = horizontalMoveDir.x;
        moveDir.z = horizontalMoveDir.z;

        characterController.Move(moveDir * Time.deltaTime);

        HandleGravityAndJump();
    }

    // с камерой немного криво работает, так как (как я понимаю), interact срабатывает когда объект
    // есть в поле зрения в целом (по крайней мере по вертикали) - это было из-за смещения камеры, у нее должны
    // быть координаты (0, 0, 0) - (только из-за этого перс сильно высокий, но это потом фиксить буду)
    // ! думаю нужно сделать указатель, типо прицел

    private void HandleInteraction() {
        // направление камеры, так как от первого лица
        Vector3 cameraDir = mainCamera.transform.forward;
        float interactDistance = 2f;

        if (Physics.Raycast(transform.position, cameraDir, out RaycastHit raycastHit, interactDistance, interactiveObjectLayer)) {
            // ! здесь нужно вызывать событие, которое при наведение на интерактивный объект будет расширять прицел 
            if (raycastHit.transform.TryGetComponent(out InteractiveObject interactiveObject)) {
                // interactiveObject
                // ! в общем нужно как-то получпть инфу об SO объекте. ЛИБО мб указывать прям в классе?
                if (interactiveObject != selectedObject) {
                    SetSelectedObject(interactiveObject);
                }
            } else {
                SetSelectedObject(null);
            }
        } else {
            SetSelectedObject(null);
        }
    }

    // ! бага - если при спавне сразу прыгнуть - звука не будет (1 раз)...
    private void HandleGravityAndJump() {
        Vector3 inputDir = inputManager.GetMovementVectorNormalized();
        if (characterController.isGrounded) {
            moveDir.y = -0.5f;  // ? зачем

            if (inputDir.y != 0) {
                // player start jump
                moveDir.y = jumpForce; // ? как сделать прыжок после бега дальше чем обычный
                playerAudio.PlayJumpStartSound();
                canPlayFallSound = true;
            }
        } else {
            moveDir.y -= gravity * Time.deltaTime;
            // способ плохо работает при перепаде высот - например при падении или спуске по лестнице
            // -> чтобы пофиксить звук я сделал флаг, теперь звук может воспроизводиться после прыжка только 1 раз
            // однако с перепадом высот все равно проблема (те же лестницы я не фиксил)
            if (moveDir.y <= -jumpForce) {
                // player land after jump
                if (canPlayFallSound) {
                    playerAudio.PlayJumpLandSound();
                    canPlayFallSound = false;
                }
            }
        }
    }

    private void HandleRotation() {
        Vector2 mouseInput = inputManager.GetMouseDelta();

        float mouseRotationX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseRotationY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        // горизонтальное вращение, поворот вокруг своей оси (то есть влево-вправо относительно Оy)
        transform.Rotate(0, mouseRotationX, 0);

        verticalRotation -= mouseRotationY; // делаем (-) для инверсии Oy
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange); // задаем ограничение на вертикальную прокрутку

        // в общем по горизонтали крутим самого игрока, по вертикали чтобы смотреть - крутим камеру
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);  // ! посмотреть тот видос про Quaternion (где все функции)
    }

    private void HandleFootsteps() {
        // думаю, что эта функция в теории может быть полезна не только для звука - по идее
        // расчет шагов пригодиться и при воспроизведении анимации перемещения

        float currentStepInterval = isSprinting ? sprintStepInterval : walkStepInterval;

        // пояснения Time.time и nextStepTime:
        // Time.time возвращает время сеанса игры, в nextStepTime записывается время, когда должен быть сделан следующий
        // шаг (то есть текущее время + интервал). Поэтому нужно сравнивать текущее время с тем, когда был сделан уже прошлый шаг
        // (когда записываем значение, оно означает след шаг, когда сравниваем - уже прошлый шаг),
        // чтобы звук воспроизводился только после того, как звук прошлого шага проиграет
        // по сути проверка: прошел ли прошлый шаг

        // пояснение characterController.velocity.magnitude и velocityThreshold:
        // characterController.velocity.magnitude - длина вектора скорости, то есть то, с какой скоростью движется объект
        // velocityThreshold - пороговое значение скорости, после достижения которого можно проигрывать звук
        
        if (characterController.isGrounded && IsPlayerMoving() && Time.time > nextStepTime && characterController.velocity.magnitude > velocityThreshold) {
            playerAudio.PlayFootstepSound();
            nextStepTime = Time.time + currentStepInterval;
        }
    }

    public void SetSelectedObject(InteractiveObject interactiveObject) {
        this.selectedObject = interactiveObject;

        OnSelectedObjectChanged?.Invoke(this, new OnSelectedObjectChangedArgs {
            selectedObject = selectedObject
        });
    }

    public InteractiveObject GetSelectedObject() {
        return selectedObject;
    }

    // public InteractiveObject GetSelectedObjectSO() {
    //     return selectedObject;
    // }

    public bool HasSelectedObject() {
        return selectedObject != null;
    }

    public bool IsPlayerMoving() {
        Vector3 inputDir = inputManager.GetMovementVectorNormalized();
        return inputDir.z != 0 || inputDir.x != 0;
    }

    public bool IsPlayerSprinting() {
        return isSprinting;
    }
}