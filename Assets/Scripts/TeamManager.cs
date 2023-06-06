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
    public List<TeamData> teams = new List<TeamData>();

    [SyncVar] public int allClients;

    [SyncVar] public int currentClients;

    public GameObject[] rects;

    [SerializeField] GameObject[] switchTeamButtons;

    bool done;

    [SyncVar]
    public List<GameObject> players = new();
    public void JointTeamBtn(int teamInt)
    {
        int id = InstanceFinder.ClientManager.Connection.ClientId;
        
        JoinTeam(teamInt, id);
    }

    
    [ServerRpc(RequireOwnership = false)]
    public void JoinTeam(int teamInt, int localPlayerId)
    {
        //print(localPlayerId);

        for (int y = 0; y < teams.Count; y++)
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
                                    players[ji].GetComponent<PlayerData>().UI.transform.SetParent(rects[teamInt].transform);
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
                                        return;
                                    }
                                }
                            }

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
                                    players[ji].GetComponent<PlayerData>().UI.transform.SetParent(rects[teamInt].transform);
                                    StartCoroutine(WaitYouDipshit());
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    [ObserversRpc] public void SetTeam(GameObject data, int TeamInt)
    {
        teams[TeamInt].tData.Add(data.GetComponent<PlayerData>());
        teams[data.GetComponent<PlayerData>().teamID].tData.Remove(data.GetComponent<PlayerData>());

    }
    public IEnumerator WaitYouDipshit()
    {
        yield return new WaitForSeconds(0.1f);
        SetParents();
    }
    [ObserversRpc] public void SetParents()
    {
        for (int x = 0; x < players.Count; x++)
        {
            print(players[x].GetComponent<PlayerData>().UI + " team id || " + x + " uiPlayers X");
            players[x].transform.GetComponent<PlayerData>().UI.transform.SetParent(rects[players[x].GetComponent<PlayerData>().teamID].transform);
        }
    }


    [ServerRpc(RequireOwnership = false)] public void AddTeam(PlayerData player, int Team)
    {
        teams[Team].tData.Add(player);
        print(teams[Team].tData + " " + Team);
        printer(Team);
    }

    [ServerRpc(RequireOwnership = false)] public void ParentPlayerUIServer(int team)
    {
        print("setparentserver");
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<PlayerData>().UI.transform.SetParent(rects[players[i].GetComponent<PlayerData>().teamID].transform);
        }
        ParentPlayerUIObserver(team);
    }
    [ObserversRpc] public void ParentPlayerUIObserver(int team)
    {
        print("setparentobserver");
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<PlayerData>().UI.transform.SetParent(rects[players[i].GetComponent<PlayerData>().teamID].transform);
        }
        
    }

    [ObserversRpc] public void SetTeamSwitchButtons()
    {
        print(LocalConnection.ClientId);
        for (int i = 0; i < players.Count; i++)
        {
            print("For I loop");
            print(players[i].GetComponent<PlayerData>().playerId);
            if (LocalConnection.ClientId == players[i].GetComponent<PlayerData>().playerId)
            {
                print("If clientId == playerId");
                for (int x = 0; x < switchTeamButtons.Length; x++)
                {
                    print("For x loop");
                    if (x == players[i].GetComponent<PlayerData>().teamID)
                    {
                        print("false");
                        switchTeamButtons[x].SetActive(false);
                    }
                    else
                    {
                        print("true");
                        switchTeamButtons[x].SetActive(true);
                    }
                }
            }
        }
        
    }

    [ServerRpc(RequireOwnership = false)] public void SetPlayerNameServer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<PlayerData>().UI.transform.GetComponentInChildren<TMP_Text>().text = players[i].GetComponent<PlayerData>().username;
        }
        SetPlayerNameObserver();
    }
    [ObserversRpc] public void SetPlayerNameObserver()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<PlayerData>().UI.transform.GetComponentInChildren<TMP_Text>().text = players[i].GetComponent<PlayerData>().username;
        }
    }
    [ObserversRpc] public void printer(int team)
    {
        print(teams[team].tData + " " + team);
    }

    public void StartGameButton()
    {
        StartGame();
    }

    public Material sky;

    [ServerRpc(RequireOwnership = false)]
    void StartGame()
    {
        SceneLoadData sld = new SceneLoadData("Game");
        SceneUnloadData lastScene = new SceneUnloadData("Lobby Test");
        base.SceneManager.LoadGlobalScenes(sld);
        base.SceneManager.UnloadGlobalScenes(lastScene);
    }
}
