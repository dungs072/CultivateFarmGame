using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using System.Linq;
[RequireComponent(typeof(ReferManagement))]
public class Fighter : NetworkBehaviour
{
    private readonly int IsAimHash = Animator.StringToHash("IsAim");
    private Weapon currentWeapon;
    private WeaponBase currentWeaponBase;
    [Header("Weapon Manager")]
    [SerializeField] private Transform weaponManager;
    [SerializeField] private Transform positionManager;
    [Header("Cinemachine")]
    [SerializeField] private Transform followTransform;
    [Header("Rig")]
    [SerializeField] private GameObject aimTargetPrefab;
    [SerializeField] private Rig rig;
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private MultiAimConstraint[] aimConstraints;
    [Header("Aim")]
    [SerializeField] private LayerMask aimColliderMask = new LayerMask();
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private float aimSpeedRotation = 20f;
    [SerializeField] private float freeSensitivity = 20f;
    [SerializeField] private float aimSensitivity = 10f;
    private CameraManagement cameraManagement;
    private CinemachineVirtualCamera aimCamera;
    private Camera mainCamera;
    private ReferManagement referManagement;
    private GameObject aimTargetObject;
    private GameObject aimTargetObjectParent;
    private float aimRigWeight;
    [Header("Temp")]
    [SerializeField] private string defaultNameWeapon = "AssaultRifle01";
    private void Start()
    {
        if(isLocalPlayer)
        {
            mainCamera = Camera.main;
            cameraMovement.SetSensitivity(freeSensitivity);
            aimRigWeight = 0f;
            cameraManagement = GameObject.FindWithTag("Camera").GetComponent<CameraManagement>();
            aimCamera = cameraManagement.CinemachineAimCamera;       
        }
        currentWeapon = Resources.Load<Weapon>(defaultNameWeapon);
        SpawnWeapon();
        SetGunTransform(currentWeaponBase.IdleGunTransform);
        aimTargetObjectParent = GameObject.FindWithTag("AimTargetParent");
        referManagement = GetComponent<ReferManagement>();
        referManagement.InputReader.OnAiming += AimMode;
        SetAimTargetForPlayer();
        SetTargetForAimConstraint();


    }
    private void OnDestroy()
    {
        referManagement.InputReader.OnAiming -= AimMode;
    }
    private void Update()
    {
        rig.weight = Mathf.Lerp(rig.weight, aimRigWeight, Time.deltaTime * 20f);
        if(referManagement.InputReader.IsAttack&&referManagement.InputReader.IsAim)
        {
            currentWeaponBase.TryShoot(aimTargetObject.transform.position);
        }  
    }

    private void AimMode(bool state)
    {
        referManagement.Animator.SetBool(IsAimHash, state);

        float rigValue = state ? 1f : 0f;
        aimRigWeight = rigValue;

        Transform temp = state ? currentWeaponBase.AimGunTransform: 
                                 currentWeaponBase.IdleGunTransform;
        SetGunTransform(temp);
        if (!isLocalPlayer) { return; }
        float sensitivity = state ? aimSensitivity : freeSensitivity;
       
        cameraMovement.SetSensitivity(sensitivity);
        aimCamera.gameObject.SetActive(state);
    }
    public void CharacterRotation()
    {
        if (mainCamera == null) { return; }
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = mainCamera.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderMask))
        {
            mouseWorldPosition = hit.point;
           
        }
        else
        {
            mouseWorldPosition = ray.GetPoint(200f);
        }
        if (!referManagement.InputReader.IsAim) { return; }
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 direction = (worldAimTarget - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime *aimSpeedRotation);
        if (aimTargetObject.transform.position == mouseWorldPosition) { return; }
        aimTargetObject.transform.position = Vector3.Lerp(aimTargetObject.transform.position, mouseWorldPosition, 
                                                    Time.deltaTime * aimSpeedRotation);
       
        if(isClientOnly)
        {
            CmdChangeAimTargetPosition(mouseWorldPosition);
        }
        else
        {
            RpcChangeAimTargetPosition(mouseWorldPosition);
        }
    }
    public void CameraRotate(float deltaTime)
    {
        cameraMovement.CameraRotate(deltaTime);
    }
    private void SetTargetForAimConstraint()
    {
       
        foreach(MultiAimConstraint aimConstraint in aimConstraints)
        {
            var data = aimConstraint.data.sourceObjects;
            data.Add(new WeightedTransform(aimTargetObject.transform, 1f));
            aimConstraint.data.sourceObjects = data;
           
        }
        rigBuilder.Build();
    }
    private void SetAimTargetForPlayer()
    {
        List<GameObject> aimTargets = GameObject.FindGameObjectsWithTag("AimTarget").ToList();
        List<GameObject> aimTargetParent = new List<GameObject>();
        for(int i =0;i<aimTargetObjectParent.transform.childCount;i++)
        {
            aimTargetParent.Add(aimTargetObjectParent.transform.GetChild(i).gameObject);
        }
        if (isLocalPlayer)
        {
            foreach (var aimTarget in aimTargets)
            {
                if (aimTarget.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    aimTargetObject = aimTarget;
                    aimTargetObject.transform.SetParent(aimTargetObjectParent.transform);
                    return;
                }
            }
        }
        else
        {
            foreach (var aimTarget in aimTargets)
            {
                if (!aimTarget.GetComponent<NetworkIdentity>().hasAuthority&&
                    !aimTargetParent.Contains(aimTarget))
                {
                    aimTargetObject = aimTarget;
                    aimTargetObject.transform.SetParent(aimTargetObjectParent.transform);
                    return;
                }
            }
        }

    }
    private void SetGunTransform(Transform newGunTransform)
    {
        currentWeaponBase.transform.position = newGunTransform.position;
        currentWeaponBase.transform.rotation = newGunTransform.rotation;
    }
    private void SpawnWeapon()
    {
        currentWeaponBase = Instantiate(currentWeapon.WeaponBase, weaponManager);
        currentWeaponBase.SetNetworkWeapon(GetComponent<HandleNetworkWeapon>());
        Transform position = Instantiate(currentWeapon.Position, positionManager);
        currentWeaponBase.IdleGunTransform = position.GetChild(0);
        currentWeaponBase.AimGunTransform = position.GetChild(1); 
    }
    [Command]
    private void CmdChangeAimTargetPosition(Vector3 position)
    {
        RpcChangeAimTargetPosition(position);
    }
    [ClientRpc]
    private void RpcChangeAimTargetPosition(Vector3 position)
    {
        if (isLocalPlayer) { return; }
        aimTargetObject.transform.position = Vector3.Lerp(aimTargetObject.transform.position, position,
                                                    Time.deltaTime * aimSpeedRotation);
    }
}
