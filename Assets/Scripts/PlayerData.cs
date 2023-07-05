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

    public TeamManager manager;

    [SyncVar] public string username;

    public GameObject UI;

    [SyncVar] public int pointsGathered;

    [SerializeField] GameObject cam;

    [SerializeField] Material tank;

    [SyncVar] public bool isReady;

    // private void Start()
    // {
    //     SpawnUIServer();
    // }
    // [ServerRpc(RequireOwnership = false)]
    // void SpawnUIServer()
    // {
    //     GameObject spawnedUI = Instantiate(UI);
    //     Spawn(spawnedUI);
    // }

    void Update()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<TeamManager>();
            manager.players.Add(this.gameObject);
            manager.currentClients++;
        }
    }

    [ObserversRpc]
    public override void OnStartClient()
    {
        base.OnStartClient();
        SetPlayerID(InstanceFinder.ClientManager.Connection.ClientId);
        SetPlayerTeam();
    }
    private void OnDestroy()
    {
        if (manager == null)
            return;
        manager.teams[teamID].tData.Remove(this);
        manager.players.Remove(this.gameObject);
        Destroy(UI);
        manager.currentClients--;
    }
    [ServerRpc(RequireOwnership = true)]
    public void SetPlayerID(int id)
    {
        playerId = id;
    }

    [ServerRpc(RequireOwnership = true)]
    public void SetPlayerTeam()
    {
        if (manager.teams[0].tData.Count - 1 <= manager.teams[1].tData.Count - 1)
        {
            teamID = 0;
            manager.AddTeam(this, teamID);
            SetParentTeam();
            GetUsernameObserver();
        }
        else
        {
            teamID = 1;
            manager.AddTeam(this, teamID);
            SetParentTeam();
            GetUsernameObserver();
        }
        StartCoroutine(manager.WaitSomeMoreDickHead());
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetParentTeam()
    {
        manager.ParentPlayerUIServer(teamID);
    }
    [ObserversRpc]
    public void GetUsernameObserver()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            GetUsernameServer(PlayerPrefs.GetString("username").ToString());
            // print("Player: " + LocalConnection.ClientId);
        }
        else
        {
            GetUsernameServer("Player: " + LocalConnection.ClientId);
        }
    }
    [ServerRpc(RequireOwnership = true)]
    public void GetUsernameServer(string name)
    {
        username = name;
        manager.SetPlayerNameServer();
    }

    [ServerRpc(RequireOwnership = false)]
    public void GainPoints(int pointAmount)
    {
        pointsGathered += pointAmount;
        //switch (pointsGathered)
        //{
        //    case 0:
        //        tank.SetFloat("_Float", -0.5f);
        //        break;
        //    case 1:
        //        tank.SetFloat("_Float", -0.1f);
        //        break;
        //    case 2:
        //        tank.SetFloat("_Float", 0.1f);
        //        break;
        //    case 3:
        //        tank.SetFloat("_Float", 0.5f);
        //        break;
        //}
    }
}