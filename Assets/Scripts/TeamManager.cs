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
    public TeamData[] teams;

    [SyncVar]
    public int allClients;

    [SyncVar]
    public int currentClients;

    public GameObject[] rects;

    public GameObject player;

    public void JointTeamBtn(int teamInt)
    {
        int id = InstanceFinder.ClientManager.Connection.ClientId;
        JoinTeam(teamInt, id);
    }
    [ServerRpc(RequireOwnership = false)]
    public void JoinTeam(int teamInt, int localPlayerId)
    {
        print("1");
        for (int i = 0; i < currentClients; i++)
        {
            print("2");


            for (int y = 0; y < teams.Length; y++)
            {
                print(y + " y");
                print(i + " i");
                if (teams[y].tData.Count == 0)
                {
                    print("teams == null" + y + " y");
                }
                else if (teams[y].tData[i].playerId == localPlayerId)
                {
                    print("3");

                    if (teams[teamInt].tData.Count <= i)
                    {

                        teams[teamInt].tData.Add(teams[y].tData[i]);
                        teams[teams[y].tData[i].teamID].tData.Remove(teams[y].tData[i]);

                        GameObject go = Instantiate(player);
                        
                        InstanceFinder.ServerManager.Spawn(go);

                        go.GetComponentInChildren<TextMeshPro>().text = "player: " + localPlayerId.ToString();
                        go.transform.SetParent(rects[teamInt].transform);
                        SetParent(go, teamInt);

                        teams[teamInt].tData[i].teamID = teamInt;
                        return;
                    }
                    else
                    {
                        for (int j = 0; j < teams[teamInt].tData.Count; i++)
                        {
                            if (teams[teamInt].tData[i] == teams[teamInt].tData[j])
                            {
                                print("same");
                                return;
                            }
                        }
                        teams[teamInt].tData.Add(teams[0].tData[i]);
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnSpectator()
    {
        GameObject go = Instantiate(player);

        InstanceFinder.ServerManager.Spawn(go);

        go.transform.SetParent(rects[0].transform);
        SetParent(go, 0);
    }

    [ObserversRpc]
    public void SetParent(GameObject go, int teamInt)
    {
        go.transform.SetParent(rects[teamInt].transform);
    }
}
