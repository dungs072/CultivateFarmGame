using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferManagement : MonoBehaviour
{
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public float AnimatorDampTime { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }

}
