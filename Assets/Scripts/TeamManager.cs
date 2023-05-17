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
    [SyncVar]
    public int currentClients;

    public int ha = 0;
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
        print(ha);
        if (InstanceFinder.ClientManager.Clients.Count != allClients)
        {
            UpdateAllPlayerDatas("pussy");
        }
    }

    [ObserversRpc]
    public void ClientCount()
    {
        ha = InstanceFinder.ClientManager.Clients.Count;
    }

    [ServerRpc(RequireOwnership = true)]
    public void UpdateAllPlayerDatas(string pussy)
    {
        for (int i = 0; i < Teams[0].tData.Count; i++)
        {
            print(pussy);
            Teams[0].tData.Clear();
            Teams[0].tData[i].SetPlayerData();
        }
        allClients = InstanceFinder.ClientManager.Clients.Count;
    }
}
