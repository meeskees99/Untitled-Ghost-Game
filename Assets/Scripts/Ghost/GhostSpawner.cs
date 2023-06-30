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

    [SyncVar][SerializeField] GameObject currentGhost;

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
        currentGhost = Instantiate(ghosts[index], transform.position, transform.rotation);
        Settrans();
        print("SpawnGhost on Spawner " + transform.name);
        currentGhost.transform.position = this.transform.position;
        Spawn(currentGhost);
        ghostManager.globalGhostPoints += ghosts[index].GetComponent<GhostMovement>().GetGhostValue();
        ghostManager.ChangeGhostAlive(1);
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
    public GameObject GetCurrentGhost()
    {
        if (currentGhost == null)
            return null;
        return currentGhost;
    }
}