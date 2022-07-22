using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class CameraMovement : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform followTransform;
    [SerializeField] private Transform lookAtTransform;

    [SerializeField] private float topClamp;
    [SerializeField] private float bottomClamp;
    [SerializeField] private float cameraAngleOverride;
    private CameraManagement cameraManagement;

    private float sensitivity;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private const float thresHold = 0.01f;

    private void Start()
    {
        if (!isLocalPlayer) { return; }
        cameraManagement = GameObject.FindWithTag("Camera").GetComponent<CameraManagement>();
        cameraManagement.CinemachineFreeCamera.Follow = followTransform;
        cameraManagement.CinemachineAimCamera.Follow = followTransform;
        cinemachineTargetYaw = followTransform.rotation.eulerAngles.y;
    }
    public void CameraRotate(float deltaTime)
    {
        if(inputReader.MouseRotateValue.sqrMagnitude>=thresHold)
        {
            cinemachineTargetYaw += inputReader.MouseRotateValue.x * deltaTime * sensitivity;
            cinemachineTargetPitch += inputReader.MouseRotateValue.y * deltaTime * sensitivity * -1;

        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw,float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);
        followTransform.rotation = Quaternion.Euler(
            cinemachineTargetPitch + cameraAngleOverride, cinemachineTargetYaw,0.0f);
    }
    public void SetSensitivity(float sensitivity)
    {
        this.sensitivity = sensitivity;
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
