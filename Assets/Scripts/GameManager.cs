using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
public class GameManager : NetworkBehaviour
{
    [SyncVar][SerializeField] PlayerData[] players;

    [SerializeField] Transform[] team1points;
    [SerializeField] Transform[] team2points;

    [SyncVar] int team1Index;
    [SyncVar] int team2Index;

    [Header("UI")]
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject tabMenu;
    [SerializeField] GameObject scoreboard1;
    [SerializeField] GameObject scoreboard2;

    [SerializeField] KeyCode inGameSettingsButton;
    [SerializeField] KeyCode scoreboardButton;
    // Start is called before the first frame update
    void Start()
    {
        if (IsHost)
        {
            SetTeamPoints();
        }
    }

    bool uiActive = false;
    void Update()
    {
        int id = InstanceFinder.ClientManager.Connection.ClientId;
        
        if (Input.GetKey(scoreboardButton) && !settingsUI.activeSelf)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerData>().playerId == id)
                {
                    if (players[i].GetComponent<PlayerData>().teamID == 0)
                    {
                        tabMenu.SetActive(true);
                        scoreboard1.SetActive(true);
                        scoreboard2.SetActive(false);
                    }
                    else if (players[i].GetComponent<PlayerData>().teamID == 1)
                    {
                        tabMenu.SetActive(true);
                        scoreboard1.SetActive(false);
                        scoreboard2.SetActive(true);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerData>().playerId == id)
                {
                    if (players[i].GetComponent<PlayerData>().teamID == 0)
                    {
                        tabMenu.SetActive(false);
                        scoreboard1.SetActive(false);
                    }
                    else if (players[i].GetComponent<PlayerData>().teamID == 1)
                    {
                        tabMenu.SetActive(false);
                        scoreboard2.SetActive(false);
                    }
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void SetTeamPoints()
    {
        players = FindObjectsOfType<PlayerData>();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].teamID == 0)
            {
                players[i].transform.position = team1points[team1Index].transform.position;
                team1Index++;
            }
            else if (players[i].teamID == 1)
            {
                players[i].transform.position = team1points[team2Index].transform.position;
                team2Index++;
            }
        }
        SetTeamPointsObserver();
    }
    [ObserversRpc(BufferLast = true)]
    void SetTeamPointsObserver()
    {
        print("a");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].teamID == 0)
            {
                players[i].transform.position = team1points[team1Index].transform.position;
            }
            else if (players[i].teamID == 1)
            {
                players[i].transform.position = team2points[team2Index].transform.position;
            }
        }
    }



}
