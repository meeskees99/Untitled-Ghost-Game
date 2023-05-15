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
    public int playerId;

    private void Start()
    {
        playerId = InstanceFinder.ClientManager.Connection.ClientId;
    }
    private void Update()
    {
        //if (!this.IsOwner)
        //{
        //    this.enabled = false;
        //}
    }
}