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
    [SyncVar] public int playerId = -2;
    [SyncVar] public int teamID;
    [SyncVar] public string username;

    public TeamManager manager;


    public GameObject UI;

    [SyncVar] public int pointsGathered;

    [SerializeField] GameObject cam;

    [SyncVar] public bool isReady;

    bool idk;

    void Start()
    {
        if (IsHost && playerId == 0)
        {

        }
        else if (IsOwner)
        {

        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SetPlayerID(InstanceFinder.ClientManager.Connection.ClientId);
    }

    private void Update()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<TeamManager>();
        }
    }
    [ServerRpc(RequireOwnership = true)]
    void SetUsername(string name)
    {
        username = name;
    }

    [ServerRpc(RequireOwnership = true)]
    void SetPlayerID(int Id)
    {
        playerId = Id;
    }










    // void Update()
    // {
    //     if (manager == null)
    //     {
    //         manager = FindObjectOfType<TeamManager>();

    //     }
    //     else if (!idk)
    //     {
    //         manager.SpawnUI(this.NetworkObject);
    //         manager.players.Add(this.gameObject);
    //         manager.currentClients++;
    //         idk = true;
    //     }
    // }

    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     SetPlayerID(InstanceFinder.ClientManager.Connection.ClientId);
    //     SetPlayerTeam();
    // }
    // private void OnDestroy()
    // {
    //     if (manager == null)
    //         return;
    //     manager.teams[teamID].tData.Remove(this);
    //     manager.players.Remove(this.gameObject);
    //     Destroy(UI);
    //     manager.currentClients--;
    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void SetPlayerID(int id)
    // {
    //     playerId = id;
    // }

    // [ServerRpc(RequireOwnership = false)]
    // public void SetPlayerTeam()
    // {
    //     if (manager.teams[0].tData.Count - 1 <= manager.teams[1].tData.Count - 1)
    //     {
    //         teamID = 0;
    //         manager.AddTeam(this, teamID);
    //         SetParentTeam();
    //         GetUsernameObserver();
    //     }
    //     else
    //     {
    //         teamID = 1;
    //         manager.AddTeam(this, teamID);
    //         SetParentTeam();
    //         GetUsernameObserver();
    //     }
    //     StartCoroutine(manager.WaitSomeMoreDickHead());
    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void SetParentTeam()
    // {
    //     manager.ParentPlayerUIServer(teamID);
    // }
    // [ObserversRpc]
    // public void GetUsernameObserver()
    // {
    //     if (PlayerPrefs.HasKey("username"))
    //     {
    //         GetUsernameServer(PlayerPrefs.GetString("username").ToString());
    //         print("Player: " + PlayerPrefs.GetString("username"));
    //     }
    //     else
    //     {
    //         GetUsernameServer("Player: " + LocalConnection.ClientId);
    //     }
    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void GetUsernameServer(string name)
    // {
    //     print("Server name = " + name);
    //     username = name;
    //     UsernameObserver(name);
    //     manager.SetPlayerNameServer();
    // }

    // public void UsernameObserver(string name)
    // {
    //     print("Obserer naem = " + name);
    //     username = name;
    // }













    // void Update()
    // {
    //     if (manager == null)
    //     {
    //         manager = FindObjectOfType<TeamManager>();

    //     }
    //     else if (!idk)
    //     {
    //         manager.SpawnUI(this.NetworkObject);
    //         manager.players.Add(this.gameObject);
    //         manager.currentClients++;
    //         idk = true;
    //     }
    // }

    // public override void OnStartClient()
    // {
    //     base.OnStartClient();
    //     SetPlayerID(InstanceFinder.ClientManager.Connection.ClientId);
    //     SetPlayerTeam();
    // }
    // private void OnDestroy()
    // {
    //     if (manager == null)
    //         return;
    //     manager.teams[teamID].tData.Remove(this);
    //     manager.players.Remove(this.gameObject);
    //     Destroy(UI);
    //     manager.currentClients--;
    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void SetPlayerID(int id)
    // {
    //     playerId = id;
    // }

    // [ServerRpc(RequireOwnership = false)]
    // public void SetPlayerTeam()
    // {
    //     if (manager.teams[0].tData.Count - 1 <= manager.teams[1].tData.Count - 1)
    //     {
    //         teamID = 0;
    //         manager.AddTeam(this, teamID);
    //         SetParentTeam();
    //         GetUsernameObserver();
    //     }
    //     else
    //     {
    //         teamID = 1;
    //         manager.AddTeam(this, teamID);
    //         SetParentTeam();
    //         GetUsernameObserver();
    //     }
    //     StartCoroutine(manager.WaitSomeMoreDickHead());
    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void SetParentTeam()
    // {
    //     manager.ParentPlayerUIServer(teamID);
    // }
    // [ObserversRpc]
    // public void GetUsernameObserver()
    // {
    //     if (PlayerPrefs.HasKey("username"))
    //     {
    //         GetUsernameServer(PlayerPrefs.GetString("username").ToString());
    //         print("Player: " + PlayerPrefs.GetString("username"));
    //     }
    //     else
    //     {
    //         GetUsernameServer("Player: " + LocalConnection.ClientId);
    //     }
    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void GetUsernameServer(string name)
    // {
    //     print("Server name = " + name);
    //     username = name;
    //     UsernameObserver(name);
    //     manager.SetPlayerNameServer();
    // }

    // public void UsernameObserver(string name)
    // {
    //     print("Obserer naem = " + name);
    //     username = name;
    // }

    [ServerRpc(RequireOwnership = true)]
    public void GainPoints(int pointAmount)
    {
        pointsGathered += pointAmount;
    }
}