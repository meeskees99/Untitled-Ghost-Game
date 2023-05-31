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

    public string username;

    bool can;
    private void Start()
    {
        playerId = -2;
        manager = FindObjectOfType<TeamManager>();
        print("joint");

        if (IsHost)
        {
            SetPlayerDataHost();
        }
        else
        {
            
            SetPlayerData();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void SetPlayerData()
    {
        manager.teams[0].tData.Add(this);
        print("server");
        teamID = 0;
        manager.currentClients++;
        manager.Username();
    }
    public void SetPlayerDataHost()
    {
        if (IsHost)
        {
            manager.HostThing(this);
        }
    }
    private void OnDestroy()
    {
        manager.teams[teamID].tData.Remove(this);
        manager.uiplayers.Remove(this.gameObject);
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

        if (PlayerPrefs.HasKey("username"))
        {
            username = PlayerPrefs.GetString("username");
        }
        else
        {
            print(playerId);
            username = "player " + playerId;
        }
    }
}