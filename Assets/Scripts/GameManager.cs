using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Ghosts")]
    [SerializeField] GameObject[] ghosts;
    int GhostIndex;
    [SerializeField] Transform spawnLocation;

    [SerializeField] PlayerData[] players;

    [SerializeField] Transform[] team1points;
    [SerializeField] Transform[] team2points;

    [SyncVar] GameObject ghost;

    [SyncVar] int team1Index;
    [SyncVar] int team2Index;
    // Start is called before the first frame update
    void Start()
    {
        if (IsHost && ghost == null)
        {
            SpawnAgent();
            players = FindObjectsOfType<PlayerData>();
            SetTeamPoints();
        }
    }

    [ServerRpc(RequireOwnership = false)] void SetTeamPoints()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].teamID == 0)
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
    [ObserversRpc(BufferLast = true)] void SetTeamPointsObserver()
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
    

    [ServerRpc(RequireOwnership = false)] void SpawnAgent()
    {
        ghost = Instantiate(ghosts[GhostIndex], spawnLocation.position, spawnLocation.rotation);
        Spawn(ghost);
    }
}
