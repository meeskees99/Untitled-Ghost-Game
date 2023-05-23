using FishNet;
using FishNet.Managing.Client;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public List<PlayerData> playerData = new List<PlayerData>();
    public TMP_Text[] clients;
    private void Update()
    {
        for (int i = 0; i < clients.Length; i++)
        {
            clients[i].text = InstanceFinder.ClientManager.Clients.Count.ToString();
        }
        
        for (int bi = 0;  bi < InstanceFinder.ClientManager.Connection.ClientId; bi++)
        {
            playerData.Add(FindObjectOfType<PlayerData>());
        }
    }
}
