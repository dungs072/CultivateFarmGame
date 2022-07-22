using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameObject aimTargetPrefab;
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        GameObject aimTargetInstance = Instantiate(aimTargetPrefab);
        NetworkServer.Spawn(aimTargetInstance, conn);
    }
}
