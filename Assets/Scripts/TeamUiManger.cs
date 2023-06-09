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
        }
        SetPlayerNamesObserver();
    }
    [ObserversRpc]
    public void SetPlayerNamesObserver()
    {
        for (int i = 0; i < players.Length; i++)
        {
            playerTxt[i].text = players[i].GetComponent<PlayerData>().username;
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
            pointsTxt[i].text = players[i].GetComponent<PlayerData>().pointsGathered.ToString();
        }
        SetPointsObserver();
    }
    [ObserversRpc]
    public void SetPointsObserver()
    {
        for (int i = 0; i < players.Length; i++)
        {
            pointsTxt[i].text = players[i].GetComponent<PlayerData>().pointsGathered.ToString();
        }
    }
}
