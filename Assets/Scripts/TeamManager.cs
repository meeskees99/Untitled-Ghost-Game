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
    public void JointTeamBtn(int teamInt)
    {
        int id = InstanceFinder.ClientManager.Connection.ClientId;
        JoinTeam(teamInt, id);
    }

    private void Start()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinTeam(int teamInt, int localPlayerId)
    {
        print("1");
        for (int i = 0; i < currentClients; i++)
        {
            print("2");


            for (int y = 0; y < Teams.Length; y++)
            {
                print(y + " y");
                print(i + " i");
                if (Teams[y].tData.Count == 0)
                {
                    print("teams == null" + y + " y");
                }
                else if (Teams[y].tData[i].playerId == localPlayerId)
                {
                    print("3");

                    if (Teams[teamInt].tData.Count <= i)
                    {
                        Teams[teamInt].tData.Add(Teams[y].tData[i]);
                        Teams[Teams[y].tData[i].TeamId].tData.Remove(Teams[y].tData[i]);
                        Teams[teamInt].tData[i].TeamId = teamInt;
                        return;
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
    }
}
