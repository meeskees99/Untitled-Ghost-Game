using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing.Logging;
using TMPro;
using FishNet;
using FishNet.Component.Spawning;

public class TeamManager : NetworkBehaviour
{
    public TeamData[] Teams;
    public int allClients;
    public void JoinTeam(int teamInt)
    {
        for (int i = 0; i < InstanceFinder.ClientManager.Clients.Count; i++)
        {
            if (Teams[0].tData[i].playerId == InstanceFinder.ClientManager.Connection.ClientId)
            {
                Teams[teamInt].tData.Add(Teams[0].tData[i]);
            }
        }
        
    }

    private void Update()
    {
        if (InstanceFinder.ServerManager.Clients.Count != allClients)
        {
            AddPlayersToTeamSpectator();
            allClients = InstanceFinder.ServerManager.Clients.Count;
        }
    }
    public void AddPlayersToTeamSpectator()
    {
        Teams[0].tData.Clear();
        Teams[0].tData.Add(FindObjectOfType<PlayerData>());
    }
}
