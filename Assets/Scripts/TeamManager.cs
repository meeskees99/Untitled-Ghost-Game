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
                if (teams[y].tData.Count == 0 && !teams[y].tData.Any())
                {
                    print("teams == null" + y + " y");
                }
                else if (teams[y].tData[i].playerId == localPlayerId)
                {
                    if (teams[teamInt].tData.Count <= i)
                    {
                        // set this in ui manager

                        print(teamInt);
                        print(y);
                        print(i);
                        teams[teamInt].tData.Add(teams[y].tData[i]);
                        teams[teams[y].tData[i].teamID].tData.Remove(teams[y].tData[i]);


                        for (int yi = 0; yi < teams[teamInt].tData.Count; yi++)
                        {
                            if (teams[teamInt].tData[yi].playerId == localPlayerId)
                            {
                                teams[teamInt].tData[yi].teamID = teamInt;
                            }
                        }

                        for (int ji = 0; ji < uiplayers.Count; ji++)
                        {
                            if (uiplayers[ji].GetComponent<PlayerData>().playerId == localPlayerId)
                            {
                                uiplayers[ji].transform.SetParent(rects[teamInt].transform);
                                StartCoroutine(WaitYouDipshit());
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
                                    print("same");
                                    return;
                                }
                            }
                        }

                        print(teamInt);
                        print(y);
                        print(i);
                        
                        teams[teamInt].tData.Add(teams[y].tData[i]);
                        
                        teams[teams[y].tData[i].teamID].tData.Remove(teams[y].tData[i]);

                        for (int yi = 0; yi < teams[teamInt].tData.Count; yi++)
                        {
                            
                            if (teams[teamInt].tData[yi].playerId == localPlayerId)
                            {
                                teams[teamInt].tData[yi].teamID = teamInt;
                            }
                        }

                        for (int ji = 0; ji < uiplayers.Count; ji++)
                        {
                            if (uiplayers[ji].GetComponent<PlayerData>().playerId == localPlayerId)
                            {
                                uiplayers[ji].transform.SetParent(rects[teamInt].transform);
                                StartCoroutine(WaitYouDipshit());
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
        for (int z = 0; z < currentClients; z++)
        {
            SetUiPlayers(uiplayers[z]);
        }
        SetParents();
    }
    [ObserversRpc]
    public void SetTeamStart(GameObject data)
    {
        teams[0].tData.Add(data.GetComponent<PlayerData>());
    }
    [ObserversRpc]
    public void SetUiPlayers(GameObject ui)
    {
        uiplayers.Add(ui);
    }
    public IEnumerator WaitYouDipshit()
    {
        yield return new WaitForSeconds(0.1f);
        print("do");
        SetParents();
    }

    [ObserversRpc]
    public void SetParents()
    {
        // set in ui manager
        for (int x = 0; x < uiplayers.Count; x++)
        {
            print(uiplayers[x].GetComponent<PlayerData>().teamID + " team id || " + x + " uiPlayers X");
            uiplayers[x].transform.SetParent(rects[uiplayers[x].GetComponent<PlayerData>().teamID].transform);

            uiplayers[x].transform.GetChild(0).GetComponent<TMP_Text>().text = uiplayers[x].GetComponent<PlayerData>().playerId.ToString();
        }
    }
}
