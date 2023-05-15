using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Managing.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Managing.Client;

public class PlayerData : NetworkBehaviour
{
    [SyncVar]
    public int playerId = -2;

    [ServerRpc(RequireOwnership = false)]
    private void Update()
    {
        if (playerId == -2)
        {
            playerId = InstanceFinder.ClientManager.Connection.ClientId;
        }
        //if (!this.IsOwner)
        //{
        //    this.enabled = false;
        //}
    }
}