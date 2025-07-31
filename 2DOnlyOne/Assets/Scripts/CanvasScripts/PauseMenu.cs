using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private ICanvasAnimator animator;

    void Awake()
    {
        animator = GetComponent<ICanvasAnimator>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Pause.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        inputActions.Player.Pause.performed -= OnPausePressed;

        inputActions.Player.Disable();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        Time.timeScale = (Time.timeScale == 1)? 0 : 1;
        animator?.Toggle();
    }
}
