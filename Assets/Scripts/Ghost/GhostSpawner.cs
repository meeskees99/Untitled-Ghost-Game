using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class GhostSpawner : NetworkBehaviour
{
    [SerializeField] GameObject[] ghosts;
    public int maxIndex;
    GameObject CurrentGhost;

    int ghostOneSpawnchance;
    int ghostTwoSpawnchance;
    int ghostThreeSpawnchance;
    // Start is called before the first frame update
    void Start()
    {
        SpawnGhost();
        StartCoroutine("Spawner");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnGhost()
    {
        int i = Random.Range(0, 100);
        if (CurrentGhost != null)
            return;
        CurrentGhost = Instantiate(ghosts[Random.Range(0 , maxIndex)]);
        Spawn(CurrentGhost);
    }
}
