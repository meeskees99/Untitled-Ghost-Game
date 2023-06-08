using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class GhostManager : NetworkBehaviour
{
    [Header("Ghosts")]
    [SerializeField] GhostSpawner[] ghostSpawner;
    // Start is called before the first frame update
    void Start()
    {
        StartSpawn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ServerRpc(RequireOwnership = false)]
    void StartSpawn()
    {
        for (int i = 0; i < ghostSpawner.Length; i++)
        {
            ghostSpawner[i].SpawnGhost();
        }
    }
}
