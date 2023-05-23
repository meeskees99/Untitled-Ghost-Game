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
using Unity.VisualScripting;

public class TeamManager : NetworkBehaviour
{
    public TeamData[] teams;

    [SyncVar]
    public int allClients;

    [SyncVar]
    public int currentClients;

    public GameObject[] rects;

    public GameObject player;

    public List<GameObject> uiplayers = new();

    public TMP_Text playernumber;
    public void JointTeamBtn(int teamInt)
    {
        int id = InstanceFinder.ClientManager.Connection.ClientId;
        
        JoinTeam(teamInt, id);
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinTeam(int teamInt, int localPlayerId)
    {
        print(localPlayerId);

        for (int i = 0; i < currentClients; i++)
        {
            for (int y = 0; y < teams.Length; y++)
            {
                //print(y + " y");
                //print(i + " i");

                if (teams[y].tData.Count == 0 && !teams[y].tData.Any())
                {
                    print("teams == null" + y + " y");
                }
                else if (teams[y].tData[i].playerId == localPlayerId)
                {
                    if (teams[teamInt].tData.Count <= i)
                    {
                        // set this in ui manager
                        teams[teamInt].tData.Add(teams[y].tData[i]);
                        teams[teams[y].tData[i].teamID].tData.Remove(teams[y].tData[i]);

                        for (int yi = 0; yi < teams[teamInt].tData.Count; yi++)
                        {
                            //print(teamInt + " teamInt");
                            if (teams[teamInt].tData[yi].playerId == localPlayerId)
                            {
                                //print("yi " + yi);
                                teams[teamInt].tData[yi].teamID = teamInt;
                            }
                        }

                        for (int ji = 0; ji < uiplayers.Count; ji++)
                        {
                            if (uiplayers[ji].GetComponent<PlayerData>().playerId == localPlayerId)
                            {
                                uiplayers[ji].transform.SetParent(rects[teamInt].transform);
                                SetParent(uiplayers[ji], teamInt);
                            }
                        }
                        
                        return;
                    }
                    else
                    {
                        if (teams[teamInt].tData[i].playerId == localPlayerId)
                        {
                            for (int j = 0; j < teams[teamInt].tData.Count; i++)
                            {
                                if (teams[teamInt].tData[i] == teams[teamInt].tData[j])
                                {
                                    print(teams[teamInt].tData[i] + " i");
                                    print(teams[teamInt].tData[i].playerId);

                                    print(teams[teamInt].tData[j] + " j");
                                    print(teams[teamInt].tData[j].playerId);
                                    print("same");
                                    return;
                                }
                            }
                        }
                        teams[teamInt].tData.Add(teams[y].tData[i]);
                        teams[teams[y].tData[i].teamID].tData.Remove(teams[y].tData[i]);

                        for (int yi = 0; yi < teams[teamInt].tData.Count; yi++)
                        {
                            //print(teamInt + " teamInt");
                            if (teams[teamInt].tData[yi].playerId == localPlayerId)
                            {
                                //print("yi " + yi);
                                teams[teamInt].tData[yi].teamID = teamInt;
                            }
                        }

                        for (int ji = 0; ji < uiplayers.Count; ji++)
                        {
                            if (uiplayers[ji].GetComponent<PlayerData>().playerId == localPlayerId)
                            {
                                uiplayers[ji].transform.SetParent(rects[teamInt].transform);
                                SetParent(uiplayers[ji], teamInt);
                            }
                        }
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnSpectator(GameObject ui)
    {
        // set in ui manager
        uiplayers.Add(ui);
        ui.transform.SetParent(rects[0].transform);
        SetParent(ui, 0);
    }

    [ObserversRpc(BufferLast = true)]
    public void SetParent(GameObject go, int teamInt)
    {
        // set in ui manager
        go.transform.SetParent(rects[teamInt].transform);

        go.transform.GetChild(0).GetComponent<TMP_Text>().text = go.GetComponent<PlayerData>().playerId.ToString();
    }
}
