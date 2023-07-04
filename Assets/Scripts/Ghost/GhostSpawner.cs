using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class GhostSpawner : NetworkBehaviour
{
    [SerializeField] GhostManager ghostManager;
    [SerializeField] GameObject[] ghosts;
    [SerializeField] float ghostSpawnChance;
    [SerializeField] float[] typeGhostChance;
    [SerializeField] int[] ghostFavor;

    [SyncVar] public GameObject currentGhost;

    [ServerRpc(RequireOwnership = false)]
    public void PickGhost()
    {
        if (CalculateSpawnChance() <= ghostSpawnChance)
        {
            if (typeGhostChance[ghostFavor[0]] > CalculateSpawnChance())
            {
                SpawnGhost(ghostFavor[0]);
            }
            else if (typeGhostChance[ghostFavor[1]] > CalculateSpawnChance())
            {
                SpawnGhost(ghostFavor[1]);
            }
            else if (typeGhostChance[ghostFavor[2]] > CalculateSpawnChance())
            {
                SpawnGhost(ghostFavor[2]);
            }
            else
            {
                SpawnGhost(0);
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnGhost(int index)
    {
        //print(this.gameObject.name);
        if (currentGhost == null)
        {
            currentGhost = Instantiate(ghosts[index], transform.position, transform.rotation);
            Settrans();
            // print("SpawnGhost on Spawner " + transform.name);
            currentGhost.transform.position = this.transform.position;
            Spawn(currentGhost);
            ghostManager.ChangeGhostAlive(1);
            ghostManager.ChangeGhostPoint(currentGhost.GetComponent<GhostMovement>().ghostData.points);
        }
    }
    [ObserversRpc]
    public void Settrans()
    {
        currentGhost.transform.position = this.transform.position;
    }

    float CalculateSpawnChance()
    {
        return Random.Range(1, 100);
    }
}