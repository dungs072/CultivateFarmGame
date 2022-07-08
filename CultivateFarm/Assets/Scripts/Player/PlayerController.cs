using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Movement movement;
    [SerializeField] private Transform followTransform;
    [SerializeField] private Transform lookAtTransform;
    private CinemachineFreeLook freeLookCamera;
    private void Start()
    {
        if (!isLocalPlayer) { return; }
        freeLookCamera = GameObject.FindWithTag("Camera").GetComponent<CinemachineFreeLook>();
        freeLookCamera.Follow = followTransform;
        freeLookCamera.LookAt = lookAtTransform;
    }
    private void Update()
    {
        movement.UpdateAnimationMovement(Time.deltaTime);
        if (!isLocalPlayer) { return; }
        Move();
    }

    private void Move()
    {
        movement.MoveTo(Time.deltaTime);
    }

  
}
