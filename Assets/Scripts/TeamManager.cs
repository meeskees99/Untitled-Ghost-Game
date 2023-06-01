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
using UnityEngine.SceneManagement;
using FishNet.Managing.Scened;

public class TeamManager : NetworkBehaviour
{
    public TeamData[] teams;

    [SyncVar]
    public int allClients;

    [SyncVar]
    public int currentClients;

    public GameObject[] rects;

    [SyncVar]
    public List<GameObject> players = new();

    public NetworkConnection[] conns;


    public TMP_Text playernumber;
    public void JointTeamBtn(int teamInt)
    {
        int id = InstanceFinder.ClientManager.Connection.ClientId;
        
        JoinTeam(teamInt, id);
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinTeam(int teamInt, int localPlayerId)
    {
        //print(localPlayerId);

        for (int y = 0; y < teams.Length; y++)
        {
            for (int i = 0; i < currentClients; i++)
            {
                if (teams[y].tData.Count - 1 < i)
                {
                    //print("NO");
                }
                else
                {
                    //print(y + " team");
                    //print(i + " player");
                    //print(teams[y].tData[i].playerId);
                    if (teams[y].tData.Count == 0 && !teams[y].tData.Any())
                    {
                        //print("teams == null" + y + " y");
                    }
                    else if (teams[y].tData[i].playerId == localPlayerId)
                    {
                        if (teams[teamInt].tData.Count <= i)
                        {
                            // set this in ui manager

                            //print(teamInt);
                            //print(y);
                            //print(i);
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

                            for (int ji = 0; ji < players.Count; ji++)
                            {
                                if (players[ji].GetComponent<PlayerData>().playerId == localPlayerId)
                                {
                                    players[ji].transform.SetParent(rects[teamInt].transform);
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
                                        //print("same");
                                        return;
                                    }
                                }
                            }

                            //print(teamInt);
                            //print(y);
                            //print(i);

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

                            for (int ji = 0; ji < players.Count; ji++)
                            {
                                if (players[ji].GetComponent<PlayerData>().playerId == localPlayerId)
                                {
                                    players[ji].transform.SetParent(rects[teamInt].transform);
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
    public void SpawnSpectator(GameObject player)
    {
        // set in ui manager
        players.Add(player);
        player.GetComponent<PlayerData>().UI.transform.SetParent(rects[0].transform);
        ClearUiPlayers();
        ClearTeamStart();
        for (int z = 0; z < currentClients; z++)
        {
            SetTeamStart(players[z]);
            SetPlayers(players[z]);
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
    public void SetPlayers(GameObject player)
    {
        players.Add(player);
    }
    [ObserversRpc]
    public void ClearUiPlayers()
    {
        players.Clear();
    }
    public IEnumerator WaitYouDipshit()
    {
        yield return new WaitForSeconds(0.1f);
        //print("do");
        SetParents();
    }

    [ObserversRpc]
    public void SetParents()
    {
        // set in ui manager
        for (int x = 0; x < players.Count; x++)
        {
            print(players[x].GetComponent<PlayerData>().UI + " team id || " + x + " uiPlayers X");
            players[x].transform.GetComponent<PlayerData>().UI.transform.SetParent(rects[players[x].GetComponent<PlayerData>().teamID].transform);
        }
    }
    public bool can;
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
            if (currentClients -1 >= players.Count)
                return;
            //print(i + " I");
            
            players[i].GetComponent<PlayerData>().UI.GetComponentInChildren<TMP_Text>().text = players[i].GetComponent<PlayerData>().username;

            //print(uiplayers[i].GetComponent<PlayerData>().username);
            UsernameClient(i, players[i].GetComponent<PlayerData>().username);
        }
    }
    [ObserversRpc]
    public void UsernameClient(int i, string name)
    {
        players[i].GetComponent<PlayerData>().UI.GetComponentInChildren<TMP_Text>().text = name;
    }

    public void StartGame()
    {
        
        SceneLoadData sld = new SceneLoadData("Game");
        List<NetworkConnection> con = new();
        
        for (int i = 0; i <= players.Count - 1; i++)
        {
            con.Add(players[i].GetComponent<NetworkObject>().Owner);
            print(con[i]);
            conns[i] = con[i];
        }
        base.SceneManager.LoadConnectionScenes(conns, sld);

    }
            
                    
        
        
    
}
