using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Managing.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Managing.Client;
using FishNet.Connection;

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

    [SerializeField] GameObject cam;

    private void Start()
    {
        manager = FindObjectOfType<TeamManager>();

        manager.players.Add(this.gameObject);
        manager.currentClients++;
    }

    [ObserversRpc]
    public override void OnStartClient()
    {
        base.OnStartClient();
        SetPlayerID(InstanceFinder.ClientManager.Connection.ClientId);
        SetPlayerTeam();
    }

    public override void OnDespawnServer(NetworkConnection connection)
    {
        base.OnDespawnServer(connection);
        OnDestroyServer();
    }
    [ServerRpc(RequireOwnership = true)] public void OnDestroyServer()
    {
        print("Manager aanwezig");
        manager.teams[teamID].tData.Remove(this);

        print(manager.teams[teamID].tData + " " + teamID);

        Despawn(UI);

        manager.players.Remove(this.gameObject);
        manager.currentClients--;
    }

    [ServerRpc(RequireOwnership = true)] public void SetPlayerID(int id)
    {
        playerId = id;
    }

    [ServerRpc(RequireOwnership = true)] public void SetPlayerTeam()
    {
        if (manager.teams[0].tData.Count -1 <= manager.teams[1].tData.Count -1)
        {
            manager.AddTeam(this, 0);
            SetParentTeam(0);
            teamID = 0;
        }
        else
        {
            manager.AddTeam(this, 1);
            SetParentTeam(1);
            teamID = 1;
        }
    }

    [ServerRpc(RequireOwnership = false)] public void SetParentTeam(int TeamID)
    {
        manager.ParentPlayerUIServer(TeamID);
    }
}