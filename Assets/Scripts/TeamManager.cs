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

    [SyncVar]
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
                        SetTeam(teamInt, y, i);
                        teams[teams[y].tData[i].teamID].tData.Remove(teams[y].tData[i]);

                        for (int yi = 0; yi < teams[teamInt].tData.Count; yi++)
                        {
                            //print(teamInt + " teamInt");
                            if (teams[teamInt].tData[yi].playerId == localPlayerId)
                            {
                                //print("yi " + yi);
                                SetTeamID(teamInt, yi);
                            }
                        }

                        for (int ji = 0; ji < uiplayers.Count; ji++)
                        {
                            if (uiplayers[ji].GetComponent<PlayerData>().playerId == localPlayerId)
                            {
                                uiplayers[ji].transform.SetParent(rects[teamInt].transform);
                                SetParents();
                            }
                        }
                        
                        return;
                    }
                    else
                    {
                        if (teams[teamInt].tData[i].playerId == localPlayerId)
                        {
                            for (int j = 0; j < teams[teamInt].tData.Count; j++)
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
                        SetTeam(teamInt, y, i);
                        teams[teams[y].tData[i].teamID].tData.Remove(teams[y].tData[i]);

                        for (int yi = 0; yi < teams[teamInt].tData.Count; yi++)
                        {
                            print(teamInt + " teamInt");
                            if (teams[teamInt].tData[yi].playerId == localPlayerId)
                            {
                                print("yi " + yi);
                                SetTeamID(teamInt, yi);
                            }
                        }

                        for (int ji = 0; ji < uiplayers.Count; ji++)
                        {
                            if (uiplayers[ji].GetComponent<PlayerData>().playerId == localPlayerId)
                            {
                                uiplayers[ji].transform.SetParent(rects[teamInt].transform);
                                SetParents();
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
        SetTeamStart(ui);
        ui.transform.SetParent(rects[0].transform);
        SetUiPlayers(ui);
        SetParents();
    }
    [ObserversRpc(BufferLast = true)]
    public void SetTeam(int Teamint, int previousteam, int dataint)
    {
        print("teamSet");
        teams[Teamint].tData.Add(teams[previousteam].tData[dataint]);
        teams[teams[Teamint].tData[dataint].teamID].tData.Remove(teams[previousteam].tData[dataint]);
    }
    [ObserversRpc]
    public void SetTeamStart(GameObject data)
    {
        teams[0].tData.Add(data.GetComponent<PlayerData>());
    }
    [ObserversRpc(BufferLast = true)]
    public void SetUiPlayers(GameObject ui)
    {
        uiplayers.Add(ui);
    }

    [ObserversRpc(BufferLast = true)]
    public void SetTeamID(int teamInt, int dataInt)
    {
        print("teamIDSet");
        print(teamInt);
        teams[teams[teamInt].tData[dataInt].teamID].tData[dataInt].teamID = teamInt;
    }

    [ObserversRpc(BufferLast = true)]
    public void SetParents()
    {
        // set in ui manager
        

        for (int x = 0; x < uiplayers.Count; x++)
        {
            print(rects[uiplayers[x].GetComponent<PlayerData>().teamID]);
            uiplayers[x].transform.SetParent(rects[uiplayers[x].GetComponent<PlayerData>().teamID].transform);

            uiplayers[x].transform.GetChild(0).GetComponent<TMP_Text>().text = uiplayers[x].GetComponent<PlayerData>().playerId.ToString();
        }
    }
}
