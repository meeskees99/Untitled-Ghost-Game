using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class GhostManager : NetworkBehaviour
{
    [Header("Ghosts")]
    [SerializeField] GhostSpawner[] ghostSpawner;
    [SerializeField] int ghostsAlive;
    [SerializeField] int maxGhosts;
    // Start is called before the first frame update
    void Start()
    {
        StartSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    void StartSpawn()
    {
        for (int i = 0; i < ghostSpawner.Length; i++)
        {
            if (ghostsAlive >= maxGhosts)
                return;
            ghostSpawner[i].SpawnGhost();
            ghostsAlive++;
        }
    }
}
