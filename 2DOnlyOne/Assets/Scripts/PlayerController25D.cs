using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController25D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;

    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isSprinting;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.Sprint.performed += OnSprintStarted;
        inputActions.Player.Sprint.canceled += OnSprintStopped;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Sprint.performed -= OnSprintStarted;
        inputActions.Player.Sprint.canceled -= OnSprintStopped;

        inputActions.Player.Disable();
    }

    private void Update()
    {
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f);
        transform.Translate(move * currentSpeed * Time.deltaTime, Space.World);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        isSprinting = true;
    }

    private void OnSprintStopped(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

}
