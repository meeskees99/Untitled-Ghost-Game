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

    [SyncVar]
    public string username;
    
    public GameObject UI;

    bool can;
    private void Start()
    {
        playerId = -2;
        manager = FindObjectOfType<TeamManager>();
        //print("joint");

        if (IsHost)
        {
            //print("host");
            SetPlayerDataHost();
        }
        else
        {
            //print("not host");
            SetPlayerData();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void SetPlayerData()
    {
        manager.teams[0].tData.Add(this);
        //print("server");
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
        manager.players.Remove(this.gameObject);
        manager.can = false; 
        manager.currentClients--;
    }
    private void Update()
    {
        //print("owner" + IsOwner);
        if (!IsOwner)
        {
            //print(" nah");
            return;
        }

        if (ya == false)
        {
            manager.SpawnSpectator(UI);
            ya = true;
        }

        if (playerId == -2)
        {
            //print("ID " + InstanceFinder.ClientManager.Connection.ClientId);
            SetPlayerIDServer(InstanceFinder.ClientManager.Connection.ClientId);
        }

    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerIDServer(int id)
    {
        playerId = id;

        if (playerId != -2)
        {
            GetUsername();
        }
    }

    [ObserversRpc]
    public void GetUsername()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            string name = PlayerPrefs.GetString("username");
            //print(name);
            GetUsernameServer(name);
            manager.Username();
        }
        else
        {
            //print(playerId);
            username = "player " + playerId;
            manager.Username();
        }
    }

    [ServerRpc]
    public void GetUsernameServer(string user)
    {
        //print(user);
        username = user;
    }
}