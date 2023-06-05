using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Ghosts")]
    [SerializeField] GameObject[] ghosts;
    int GhostIndex;
    [SerializeField] Transform spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        SpawnAgent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [ServerRpc(RequireOwnership = false)]
    void SpawnAgent()
    {
        GameObject ghost = Instantiate(ghosts[GhostIndex], spawnLocation.position, spawnLocation.rotation);
        Spawn(ghost);
    }
}
