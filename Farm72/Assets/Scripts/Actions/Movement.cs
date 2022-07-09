using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ReferManagement))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 10f;
    private readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    private ReferManagement referManagement;
    private void Start()
    {
        referManagement = GetComponent<ReferManagement>();
    }
    public void MoveTo(float deltaTime)
    {
       
        Vector3 movement = CalculateMovement(referManagement.InputReader.MainCameraTransform,
                                            referManagement.InputReader.MovementValue);

        bool isSpeedUp = referManagement.InputReader.IsSpeedUp;
        float speed = isSpeedUp ? runSpeed : walkSpeed;
        MoveTo(movement*speed, deltaTime);
        if (referManagement.InputReader.MovementValue == Vector2.zero) { return; }
        FaceMovementDirection(movement, deltaTime);
    }
    public void MoveTo(Vector3 motion,float deltaTime)
    {
        referManagement.Controller.Move((motion +referManagement.ForceReceiver.Movement)*deltaTime);
    }
    public void UpdateAnimationMovement(float deltaTime)
    {
        if (referManagement == null) { return; }
        Vector2 MovementValue = referManagement.InputReader.MovementValue;
        bool isSpeedUp = referManagement.InputReader.IsSpeedUp;
        if(MovementValue==Vector2.zero)
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
    private Vector3 CalculateMovement(Transform MainCameraTransform,Vector2 MovementValue)
    {
        Vector3 forward = MainCameraTransform.forward;
        Vector3 right = MainCameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        return forward *MovementValue.y +
               right *MovementValue.x;
    }
    private void FaceMovementDirection(Vector3 movement,float deltaTime)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
                                            Quaternion.LookRotation(movement),
                                            deltaTime * referManagement.RotationDamping);
    }
}
