using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;

public class TeamUiManger : NetworkBehaviour
{
    [SerializeField] List<GameObject> team1Players = new();
    [SerializeField] List<GameObject> team2Players = new();
    [SerializeField] TMP_Text[] playerTxtTeam1;
    [SerializeField] TMP_Text[] playerTxtTeam2;
    [SerializeField] TMP_Text[] pointsTxtTeam1;
    [SerializeField] TMP_Text[] pointsTxtTeam2;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsHost)
        {
            GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].GetComponent<PlayerData>().teamID == 0)
                {
                    team1Players.Add(temp[i]);
                }
                else if (temp[i].GetComponent<PlayerData>().teamID == 1)
                {
                    team2Players.Add(temp[i]);
                }
            }
        }
        SetPlayerNames();

    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNames()
    {
        for (int i = 0; i < team1Players.Count - 1; i++)
        {
            playerTxtTeam1[i].text = team1Players[i].GetComponent<PlayerData>().username;
            SetPlayerNamesObserver(team1Players[i].GetComponent<PlayerData>().username, i, 0);
        }
        for (int i = 0; i < team2Players.Count - 1; i++)
        {
            playerTxtTeam2[i].text = team2Players[i].GetComponent<PlayerData>().username;
            SetPlayerNamesObserver(team2Players[i].GetComponent<PlayerData>().username, i, 1);
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
        for (int i = 0; i < team1Players.Count - 1; i++)
        {
            playerTxtTeam1[i].text = team1Players[i].GetComponent<PlayerData>().pointsGathered.ToString();
            SetPointsObserver(team1Players[i].GetComponent<PlayerData>().pointsGathered.ToString(), i, 0);
        }
        for (int i = 0; i < team2Players.Count - 1; i++)
        {
            playerTxtTeam2[i].text = team2Players[i].GetComponent<PlayerData>().pointsGathered.ToString();
            SetPointsObserver(team2Players[i].GetComponent<PlayerData>().pointsGathered.ToString(), i, 1);
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
