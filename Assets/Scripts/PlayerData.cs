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

    [SerializeField] GameObject cam;

    int team;
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
        if (manager.teams[0].tData.Count <= manager.teams[1].tData.Count)
        {
            manager.teams[0].tData.Add(this);
            team = 0;
            manager.SpawnSpectator(this.gameObject, team);
        }
        else
        {
            manager.teams[1].tData.Add(this);
            team = 1;
            manager.SpawnSpectator(this.gameObject, team);
        }

        print("Team 1 count: " + manager.teams[0].tData.Count);
        print("Team 2 count: " + manager.teams[1].tData.Count);
        //print("server");
        teamID = 0;
        manager.currentClients++;
        manager.Username();
    }
    public void SetPlayerDataHost()
    {
        if (IsHost && playerId == 0)
        {
            manager.HostThing(this);
        }
    }
    private void OnDestroy()
    {
        if(manager == null)
            return;
        print("Manager aanwezig");
        manager.teams[teamID].tData.Remove(this);
        manager.players.Remove(this.gameObject);
        manager.can = false; 
        manager.currentClients--;
    }
    private void Update()
    {
        if (IsHost)
        {
            if (playerId != 0)
            {
                this.transform.GetComponent<MovementAdvanced>().enabled = false;
                cam.SetActive(false);
            }
            else
            {
                this.transform.GetComponent<MovementAdvanced>().enabled = true;
                cam.SetActive(true);
            }
        }
        else if (!base.IsOwner)
        {
            this.transform.GetComponent<MovementAdvanced>().enabled = false;
            cam.SetActive(false);
        }
        //print("owner" + IsOwner);
        if (!IsOwner)
        {
            //print(" nah");
            return;
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