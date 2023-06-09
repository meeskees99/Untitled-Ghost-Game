using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;

public class TeamUiManger : NetworkBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] TMP_Text[] playerTxt;
    [SerializeField] TMP_Text[] pointsTxt;

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
            playerTxt[i].text = players[i].GetComponent<PlayerData>().username;
            SetPlayerNamesObserver(players[i].GetComponent<PlayerData>().username, i);
        }
    }
    [ObserversRpc]
    public void SetPlayerNamesObserver(string usernames, int i)
    {
        playerTxt[i].text = usernames;
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
            pointsTxt[i].text = players[i].GetComponent<PlayerData>().pointsGathered.ToString();
            SetPointsObserver(players[i].GetComponent<PlayerData>().pointsGathered, i);
        }
    }
    [ObserversRpc]
    public void SetPointsObserver(int points, int i)
    {
        pointsTxt[i].text = points.ToString();
    }
}
