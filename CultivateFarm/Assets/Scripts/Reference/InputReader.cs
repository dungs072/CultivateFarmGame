using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class InputReader : NetworkBehaviour,Controllers.IPlayerActions
{
    public Transform MainCameraTransform { get; private set; }
    public Vector2 MovementValue { get { return movementValue; } }
    [SyncVar]
    private Vector2 movementValue;
    public bool IsSpeedUp { get; private set; }
    private Controllers controls;
    private void Start()
    {
        if (!isLocalPlayer) { return; }
        MainCameraTransform = Camera.main.transform;
        controls = new Controllers();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        movementValue = context.ReadValue<Vector2>();
        if (isClientOnly)
        {
            CmdSetMovementValue(movementValue);
        }
    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsSpeedUp = true;
        }
        else if (context.canceled)
        {
            IsSpeedUp = false;
        }
    }
    [Command]
    private void CmdSetMovementValue(Vector2 movementValue)
    {
        this.movementValue = movementValue;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
    }
}
