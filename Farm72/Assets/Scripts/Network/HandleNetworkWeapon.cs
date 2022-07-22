using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class HandleNetworkWeapon : NetworkBehaviour
{
    private GameObject currentProjectilePrefab;
    public void SetCurrentProjectitle(GameObject projectile)
    {
        currentProjectilePrefab = projectile;
    }
    public void SpawnProjectileOverNetwork(Vector3 target,Vector3 spawnPos)
    {
        if (!hasAuthority) { return; }
        CmdSpawnProjectile(target, spawnPos);
    }
    [Command]
    private void CmdSpawnProjectile(Vector3 target,Vector3 spawnPos)
    {
        Vector3 direction = (target - spawnPos).normalized;
        GameObject projectInstance = Instantiate(currentProjectilePrefab, spawnPos,
                            Quaternion.LookRotation(direction, Vector3.up));
        NetworkServer.Spawn(projectInstance,connectionToClient);
    }
}
