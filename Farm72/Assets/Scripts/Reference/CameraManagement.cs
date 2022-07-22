using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManagement : MonoBehaviour
{
    [field: SerializeField] public CinemachineFreeLook CinemachineFreeLookCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera CinemachineAimCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera CinemachineFreeCamera { get; private set; }
}
