using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    int team1Index;
    int team2Index;
    // Start is called before the first frame update
    void Start()
    {
        if (IsHost && ghost == null)
        {
            SpawnAgent();
            players = FindObjectsOfType<PlayerData>();
            SetTeamPoints();
        }
        team1Index= 0;
        team2Index = 0;
    }

    [ServerRpc(RequireOwnership = false)] void SetTeamPoints()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if(players[i].teamID == 0)
            {
                players[i].transform.position = team1points[team1Index].transform.position;
                print(players[i].teamID);
                SetTeamPointsObserver(i, players[i].teamID);
                print(players[i].teamID);
                team1Index++;
            }
            else if (players[i].teamID == 1)
            {
                players[i].transform.position = team1points[team2Index].transform.position;
                print(players[i].teamID);
                SetTeamPointsObserver(i, players[i].teamID);
                print(players[i].teamID);
                team2Index++;
            }
        }
    }
    [ObserversRpc] void SetTeamPointsObserver(int playerint,  int teamint)
    {
        print("a");
        if (teamint== 0)
        {
            players[playerint].transform.position = team1points[team1Index].transform.position;
        }
        else if (teamint== 1)
        {
            players[playerint].transform.position = team2points[team2Index].transform.position;
        }
    }    
    

    [ServerRpc(RequireOwnership = false)] void SpawnAgent()
    {
        ghost = Instantiate(ghosts[GhostIndex], spawnLocation.position, spawnLocation.rotation);
        Spawn(ghost);
    }
}
