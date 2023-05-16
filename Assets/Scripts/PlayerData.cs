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



    private void Start()
    {
        playerId = -2;
    }
    private void Update()
    {
        print("owner" + IsOwner);
        if (!IsOwner)
        {
            return;
        }

        if (playerId == -2)
        {
            print("ID " + InstanceFinder.ClientManager.Connection.ClientId);
            SetPlayerID(InstanceFinder.ClientManager.Connection.ClientId);
        }
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerID(int id)
    {
        playerId = id;
    }
}