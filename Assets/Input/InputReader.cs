using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "NewInputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, Controls.IPlayerActions
{
    private Controls controls;

    //Events
    public event Action<bool> PrimaryFireEvent;
    public event Action<Vector2> MoveEvent; 
    public event Action<float> RotateBodyEvent;
    public event Action<float> RotateTurretEvent;

    // ------------------ Callbacks del Input System ------------------ //
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);
        }
    }

    public void OnRotateBody(InputAction.CallbackContext context)
    {
        // Eje 1D (-1 a 1), por ejemplo Q/E o joystick horizontal
        RotateBodyEvent?.Invoke(context.ReadValue<float>());
    }

    public void OnRotateTurret(InputAction.CallbackContext context)
    {
        // Eje 1D (-1 a 1), por ejemplo I/P o joystick derecho horizontal
        RotateTurretEvent?.Invoke(context.ReadValue<float>());
    }

    // ------------------ Activar / desactivar el mapa ------------------ //
    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls?.Player.Disable();
    }
}
