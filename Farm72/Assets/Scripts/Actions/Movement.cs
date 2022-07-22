using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[RequireComponent(typeof(ReferManagement))]
public class Movement : NetworkBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 10f;
    private readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private readonly int FallHash = Animator.StringToHash("Fall");
    private readonly int ForwardHash = Animator.StringToHash("Forward");
    private readonly int RightHash = Animator.StringToHash("Right");
    private ReferManagement referManagement;
    private Vector3 momentum;
    private bool canFall;
    private bool isJumping;
    private void Start()
    {
        referManagement = GetComponent<ReferManagement>();
        referManagement.InputReader.OnJumping += Jump;

    }
    private void OnDestroy()
    {
        referManagement.InputReader.OnJumping -= Jump;
    }


    public void HandleJump()
    {
        if (!isJumping) { return; }
        if (referManagement.InputReader.IsJump)
        {
            MoveTo(momentum, Time.deltaTime);
            if (referManagement.Controller.velocity.y <= 0f)
            {
                if (canFall)
                {
                    Fall();
                    canFall = false;
                }
            }
            if (referManagement.Controller.isGrounded)
            {
                referManagement.InputReader.SetIsJump(false);
                isJumping = false;
                ReturnToLocomotion();
            }
        }
    }

    public void MoveTo(float deltaTime)
    {
        if (referManagement == null) { return; }
        Vector3 movement = CalculateMovement(referManagement.InputReader.MainCameraTransform,
                                            referManagement.InputReader.MovementValue);
        bool isSpeedUp = referManagement.InputReader.IsSpeedUp;
        float speed = isSpeedUp ? runSpeed : walkSpeed;
        MoveTo(movement * speed, deltaTime);
        if (referManagement.InputReader.MovementValue == Vector2.zero) { return; }
        FaceMovementDirection(movement, deltaTime);
    }
    public void MoveTo(Vector3 motion, float deltaTime)
    {
        referManagement.Controller.Move((motion + referManagement.ForceReceiver.Movement) * deltaTime);
    }
    public void UpdateAnimationMovement(float deltaTime)
    {
        if (referManagement == null) { return; }
        UpdateAnimationOnFreeLook(deltaTime);
        if (!referManagement.InputReader.IsAim) { return; }
        UpdateAnimationOnTargetLook(deltaTime);
    }
    private void UpdateAnimationOnFreeLook(float deltaTime)
    {
        Vector2 MovementValue = referManagement.InputReader.MovementValue;
        bool isSpeedUp = referManagement.InputReader.IsSpeedUp;
        if (MovementValue == Vector2.zero)
        {
            referManagement.Animator.SetFloat(LocomotionHash, 0f,
                    referManagement.AnimatorDampTime, deltaTime);
        }
        else
        {
            if (isSpeedUp)
            {
                referManagement.Animator.SetFloat(LocomotionHash, 1f,
                        referManagement.AnimatorDampTime, deltaTime);
            }
            else
            {
                referManagement.Animator.SetFloat(LocomotionHash, 0.5f,
                        referManagement.AnimatorDampTime, deltaTime);
            }
        }
    }
    private void UpdateAnimationOnTargetLook(float deltaTime)
    {
        Vector2 MovementValue = referManagement.InputReader.MovementValue;
        referManagement.Animator.SetFloat(ForwardHash, MovementValue.y,
                            referManagement.AnimatorDampTime,deltaTime);
        referManagement.Animator.SetFloat(RightHash, MovementValue.x,
                            referManagement.AnimatorDampTime,deltaTime);
    }
    private Vector3 CalculateMovement(Transform MainCameraTransform, Vector2 MovementValue)
    {
        Vector3 forward = MainCameraTransform.forward;
        Vector3 right = MainCameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        return forward * MovementValue.y +
               right * MovementValue.x;
    }
    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        if (referManagement.InputReader.IsAim) { return; }
        transform.rotation = Quaternion.Lerp(transform.rotation,
                                            Quaternion.LookRotation(movement),
                                            deltaTime * referManagement.RotationDamping);
    }
    private void Jump()
    {
        if (isJumping) { return; }
        referManagement.ForceReceiver.Jump(jumpForce);
        momentum = referManagement.Controller.velocity;
        momentum.y = 0f;
        isJumping = true;
        canFall = true;
        referManagement.Animator.CrossFadeInFixedTime(JumpHash, referManagement.AnimatorDampTime);
        CmdRunJumpAnimation();
    }
    private void Fall()
    {
        momentum = referManagement.Controller.velocity;
        referManagement.Animator.CrossFadeInFixedTime(FallHash, referManagement.AnimatorDampTime);
        CmdRunFallAnimation();
    }
    private void ReturnToLocomotion()
    {
        referManagement.Animator.CrossFadeInFixedTime(LocomotionHash, referManagement.AnimatorDampTime);
        CmdReturnLocomotion();
    }
    #region Server
    [Command]
    private void CmdRunJumpAnimation()
    {
        RpcRunJumpAnimation();
    }
    [Command]
    private void CmdRunFallAnimation()
    {
        RpcRunFallAnimation();
    }
    [Command]
    private void CmdReturnLocomotion()
    {
        RpcReturnLocomotion();
    }
    #endregion

    #region Client
    [ClientRpc]
    private void RpcRunJumpAnimation()
    {
        if (hasAuthority) { return; }
        referManagement.Animator.CrossFadeInFixedTime(JumpHash, referManagement.AnimatorDampTime);
    }
    [ClientRpc]
    private void RpcRunFallAnimation()
    {
        if (hasAuthority) { return; }
        referManagement.Animator.CrossFadeInFixedTime(FallHash, referManagement.AnimatorDampTime);
    }
    [ClientRpc]
    private void RpcReturnLocomotion()
    {
        if (hasAuthority) { return; }
        referManagement.Animator.CrossFadeInFixedTime(LocomotionHash, referManagement.AnimatorDampTime);
    }

    #endregion
}