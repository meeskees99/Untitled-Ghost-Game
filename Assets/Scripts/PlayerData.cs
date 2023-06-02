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

    private void Start()
    {
        manager = FindObjectOfType<TeamManager>();

        manager.players.Add(this.gameObject);
    }
    private void OnDestroy()
    {
        if(manager == null)
            return;

        print("Manager aanwezig");
        manager.teams[teamID].tData.Remove(this);
        manager.players.Remove(this.gameObject);
        manager.currentClients--;
    }
    private void Update()
    {
        
    }
}