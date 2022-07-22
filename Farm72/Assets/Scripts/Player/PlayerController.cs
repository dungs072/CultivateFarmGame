using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Movement movement;
    [SerializeField] private Fighter fighter;
  

    private CameraManagement cameraManagement;

    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private bool captureCursor;
    private void Start()
    {
        SetCursorState(true);
    }
    private void Update()
    {
        movement.UpdateAnimationMovement(Time.deltaTime);
        
        if (!isLocalPlayer) { return; }
        HandlePressEscapeKey();
        HandleMove();
        HandleJump();
        HandleAimRotation();
        
    }
    private void LateUpdate()
    {
        fighter.CameraRotate(Time.deltaTime);
    }
    private void HandlePressEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorState(captureCursor);
            captureCursor = !captureCursor;
        }
    }

    private void HandleMove()
    {
        movement.MoveTo(Time.deltaTime);
    }
    private void HandleAimRotation()
    {
        fighter.CharacterRotation();
    }
    private void HandleJump()
    {
        movement.HandleJump();
    }
    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
    

}
