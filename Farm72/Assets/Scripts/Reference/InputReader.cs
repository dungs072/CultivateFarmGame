using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System;
public class InputReader : NetworkBehaviour, Controllers.IPlayerActions
{
    public event Action<bool> OnAiming;
    public event Action OnJumping;
    public Transform MainCameraTransform { get; private set; }
    public Vector2 MovementValue { get { return movementValue; } }
    [SyncVar]
    private Vector2 movementValue;
    public bool IsSpeedUp { get { return isSpeedUp; } }
    [SyncVar]
    private bool isSpeedUp;
    public bool IsAim { get { return isAim; } }
    [SyncVar(hook = nameof(OnChangeAim))]
    private bool isAim;
    public bool IsAttack { get { return isAttack; } }
    [SyncVar]
    private bool isAttack;
    public bool IsJump { get { return isJump; } }
    private bool isJump;
    public Vector2 MouseRotateValue { get; set; }
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
        if (!isActiveAndEnabled) { return; }
        if (!hasAuthority) { return; }
        movementValue = context.ReadValue<Vector2>();
        if (isClientOnly)
        {
            if (!isActiveAndEnabled) { return; }
            CmdSetMovementValue(movementValue);
        }
    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (!isActiveAndEnabled) { return; }
        if (!hasAuthority) { return; }
        if (context.performed)
        {
            CmdSetCanSpeedUp(true);
        }
        else if (context.canceled)
        {
            CmdSetCanSpeedUp(false);
        }
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        if (!isActiveAndEnabled) { return; }
        MouseRotateValue = context.ReadValue<Vector2>();
    }
    public void OnAim(InputAction.CallbackContext context)
    {
        if (!isActiveAndEnabled) { return; }
        if (context.performed)
        {
            isAim = !isAim;
            if (isClientOnly)
            {
                CmdSetAiming(isAim);
            }
            OnAiming?.Invoke(isAim);
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!isActiveAndEnabled) { return; }
        if (context.performed)
        {
            CmdSetAttacking(true);
        }
        else if (context.canceled)
        {
            CmdSetAttacking(false);
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            if (!hasAuthority) { return; }
            OnJumping?.Invoke();
            isJump = true;
        }

    }
    private void OnChangeAim(bool oldValue, bool newValue)
    {
        if (isLocalPlayer) { return; }
        OnAiming?.Invoke(isAim);
    }
    public void SetIsJump(bool state)
    {
        isJump = state;
    }
    #region Server
    [Command]
    private void CmdSetMovementValue(Vector2 movementValue)
    {
        this.movementValue = movementValue;
    }
    [Command]
    private void CmdSetCanSpeedUp(bool isSpeedUp)
    {
        this.isSpeedUp = isSpeedUp;
    }
    [Command]
    private void CmdSetAiming(bool isAim)
    {
        this.isAim = isAim;
    }
    [Command]
    private void CmdSetAttacking(bool isAttack)
    {
        this.isAttack = isAttack;
    }
    [Command]
    private void CmdSetIsJump(bool isJump)
    {
        this.isJump = isJump;
    }
    [Command]
    private void CmdOnJump()
    {
        RpcOnJumpEvent();
    }
    #endregion

    #region Client
    [ClientRpc]
    private void RpcOnJumpEvent()
    {

        if (hasAuthority) { return; }
        OnJumping?.Invoke();
        isJump = true;
    }
    #endregion
}
