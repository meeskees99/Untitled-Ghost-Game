using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing.Logging;
using TMPro;
using FishNet;
using FishNet.Component.Spawning;
using System.Linq;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class TeamManager : NetworkBehaviour
{
    public TeamData[] Teams;

    [SyncVar]
    public int allClients;


    public void JointTeamBtn(int teamInt)
    {
        int id = InstanceFinder.ClientManager.Connection.ClientId;
        JoinTeam(teamInt, id);
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinTeam(int teamInt, int localPlayerId)
    {
        print("1");
        for (int i = 0; i < allClients; i++)
        {
            print("2");
            print("i" + i);
            if (Teams[0].tData[i].playerId == localPlayerId)
            {
                print("3");
                // only if tdata.count is 0
                if (Teams[teamInt].tData.Count <= i)
                {
                    Teams[teamInt].tData.Add(Teams[0].tData[i]);
                }
                else
                {
                    for (int j = 0; j < Teams[teamInt].tData.Count; i++)
                    {
                        if (Teams[teamInt].tData[i] == Teams[teamInt].tData[j])
                        {
                            print("same");
                            return;
                        }
                    }
                    Teams[teamInt].tData.Add(Teams[0].tData[i]);
                }
            }
        }
        
    }

    private void Update()
    {
        print(InstanceFinder.ServerManager.Clients.Count);
        if (InstanceFinder.ServerManager.Clients.Count != allClients || Input.GetKeyDown(KeyCode.U))
        {
            print("ha");
            AddPlayersToTeamSpectator();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayersToTeamSpectator(NetworkConnection conn = null)
    {
        print("babaBOO");
        Teams[0].tData.Clear();
        print(FindObjectOfType<PlayerData>());
        for (int i = 0; i < allClients; i++)
        {
            if (Teams[0].tData.Count <= i)
            {
                Teams[0].tData.Add(FindObjectOfType<PlayerData>());
            }
            else
            {
                if (Teams[0].tData[i] == FindObjectOfType<PlayerData>())
                {
                    allClients = InstanceFinder.ServerManager.Clients.Count;
                    return;
                }
                else
                {
                    Teams[0].tData.Add(FindObjectOfType<PlayerData>());
                }
            }
        }
        allClients = InstanceFinder.ServerManager.Clients.Count;
    }
}
