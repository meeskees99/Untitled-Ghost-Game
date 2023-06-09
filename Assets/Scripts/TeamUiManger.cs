using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;

public class TeamUiManger : NetworkBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] TMP_Text[] playerTxtTeam1;
    [SerializeField] TMP_Text[] playerTxtTeam2;
    [SerializeField] TMP_Text[] pointsTxtTeam1;
    [SerializeField] TMP_Text[] pointsTxtTeam2;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsHost)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        SetPlayerNames();

    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNames()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerData>().teamID == 0)
            {
                playerTxtTeam1[i].text = players[i].GetComponent<PlayerData>().username;
                SetPlayerNamesObserver(players[i].GetComponent<PlayerData>().username, i, 0);
            }
            else
            {
                playerTxtTeam2[i].text = players[i].GetComponent<PlayerData>().username;
                SetPlayerNamesObserver(players[i].GetComponent<PlayerData>().username, i, 1);
            }
        }
    }
    [ObserversRpc]
    public void SetPlayerNamesObserver(string usernames, int i, int team)
    {
        if (team == 0)
        {
            playerTxtTeam1[i].text = usernames;
        }
        else if (team == 1)
        {
            playerTxtTeam2[i].text = usernames;
        }

    }
    // Update is called once per frame
    void Update()
    {
        SetPoints();
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPoints()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerData>().teamID == 0)
            {
                playerTxtTeam1[i].text = players[i].GetComponent<PlayerData>().pointsGathered.ToString();
                SetPointsObserver(players[i].GetComponent<PlayerData>().pointsGathered.ToString(), i, 0);
            }
            else if (players[i].GetComponent<PlayerData>().teamID == 1)
            {
                playerTxtTeam2[i].text = players[i].GetComponent<PlayerData>().pointsGathered.ToString();
                SetPointsObserver(players[i].GetComponent<PlayerData>().pointsGathered.ToString(), i, 1);
            }

        }
    }
    [ObserversRpc]
    public void SetPointsObserver(string points, int i, int team)
    {
        if (team == 0)
        {
            pointsTxtTeam1[i].text = points;
        }
        else if (team == 1)
        {
            pointsTxtTeam2[i].text = points;
        }
    }
}
