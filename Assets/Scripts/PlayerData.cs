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

    [SyncVar]
    public int teamID;

    public TeamManager manager;
    bool ya;
    private void Start()
    {
        playerId = -2;
        manager = FindObjectOfType<TeamManager>();
        
        SetPlayerData();
    }

    [ServerRpc(RequireOwnership = true)]
    public void SetPlayerData()
    {
        manager.teams[0].tData.Add(this);
        
        teamID = 0;
        manager.currentClients++;
    }
    private void OnDestroy()
    {
        manager.teams[0].tData.Remove(this);
        manager.currentClients--;
    }
    private void Update()
    {
        //print("owner" + IsOwner);
        if (!IsOwner)
        {
            return;
        }

        if (playerId == -2)
        {
            print("ID " + InstanceFinder.ClientManager.Connection.ClientId);
            SetPlayerIDServer(InstanceFinder.ClientManager.Connection.ClientId);
        }
        
        if (ya == false)
        {
            manager.SpawnSpectator(this.gameObject);
            ya = true;  
        }
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerIDServer(int id)
    {
        playerId = id;
        SetPlayerIDClients(id);
    }
    [ObserversRpc(BufferLast = true)]
    public void SetPlayerIDClients(int id)
    {
        manager.playernumber.text = playerId.ToString();
        playerId = id;
    }
}