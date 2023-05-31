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

        for (int y = 0; y < teams.Length; y++)
        {
            for (int i = 0; i < currentClients; i++)
            {
                if (teams[y].tData.Count - 1 < i)
                {
                    print("NO");
                }
                else
                {
                    print(y + " team");
                    print(i + " player");
                    print(teams[y].tData[i].playerId);
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
                            SetTeam(teams[y].tData[i].gameObject, teamInt);
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
                            SetTeam(teams[y].tData[i].gameObject, teamInt);
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
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnSpectator(GameObject ui)
    {
        // set in ui manager
        uiplayers.Add(ui);
        ui.transform.SetParent(rects[0].transform);
        ClearUiPlayers();
        ClearTeamStart();
        for (int z = 0; z < currentClients; z++)
        {
            SetTeamStart(uiplayers[z]);
            SetUiPlayers(uiplayers[z]);
        }
        SetParents();
    }
    [ObserversRpc]
    public void SetTeamStart(GameObject data)
    {
        teams[data.GetComponent<PlayerData>().teamID].tData.Add(data.GetComponent<PlayerData>());
    }
    [ObserversRpc]
    public void ClearTeamStart()
    {
        teams[0].tData.Clear();
        teams[1].tData.Clear();
        teams[2].tData.Clear();
    }
    [ObserversRpc]
    public void SetTeam(GameObject data, int TeamInt)
    {
        teams[TeamInt].tData.Add(data.GetComponent<PlayerData>());
        teams[data.GetComponent<PlayerData>().teamID].tData.Remove(data.GetComponent<PlayerData>());

    }
    [ObserversRpc]
    public void SetUiPlayers(GameObject ui)
    {
        uiplayers.Add(ui);
    }
    [ObserversRpc]
    public void ClearUiPlayers()
    {
        uiplayers.Clear();
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
        }
    }
    bool can;
    public void HostThing(PlayerData data)
    {
        if (!can)
        {
            if (PlayerPrefs.HasKey("username"))
            {
                data.username = PlayerPrefs.GetString("username");
            }
            else
            {
                data.username = "player " + data.playerId;
            }
            can = true;
            teams[0].tData.Add(data.GetComponent<PlayerData>());
            currentClients++;
            Username();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void Username()
    {
        for (int i = 0; i <= currentClients -1; i++)
        {
            if (currentClients -1 >= uiplayers.Count)
                return;
            print(i + " I");
            
            uiplayers[i].GetComponentInChildren<TMP_Text>().text = uiplayers[i].GetComponent<PlayerData>().username;
            UsernameClient(i);
        }
    }
    [ObserversRpc]
    public void UsernameClient(int i)
    {
        uiplayers[i].GetComponentInChildren<TMP_Text>().text = uiplayers[i].GetComponent<PlayerData>().username;
    }
}
