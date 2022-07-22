using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform spawnProjectilePos;
    [SerializeField] private BoxCollider boxCollider;
   
    [Header("Value")]
    [SerializeField] private float timeLockFire =0.1f;

    public Transform IdleGunTransform { get; set; }
    public Transform AimGunTransform { get; set; }
    
    private HandleNetworkWeapon networkWeapon;
    private bool isLockFire;
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        isLockFire = false;
        boxCollider.enabled = false;
        
    }
    public void TryShoot(Vector3 target)
    {
        if (isLockFire) { return; }
        StartCoroutine(Shoot(target));
    }
    private IEnumerator Shoot(Vector3 target)
    {
        isLockFire = true;
        networkWeapon.SpawnProjectileOverNetwork(target, spawnProjectilePos.position);
        yield return new WaitForSeconds(timeLockFire);
        isLockFire = false;
    }
    public void SetNetworkWeapon(HandleNetworkWeapon networkWeapon)
    {
        this.networkWeapon = networkWeapon;
        networkWeapon.SetCurrentProjectitle(projectilePrefab.gameObject);
    }
}
