using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using FishNet.Object;
public class GhostManager : NetworkBehaviour
{
    [Header("GhostSpawners settings")]
    [SerializeField] GhostSpawner[] ghostSpawners;
    [SerializeField] int ghostsAlive;
    [SerializeField] int maxGhosts;
    public int globalGhostPoints;

    [SerializeField] List<GhostSpawner> availableSpawners = new();

    [SerializeField] float spawnDelay = 1f;
    [SyncVar][SerializeField] bool isStarted;

    public void Start()
    {
        if (IsHost && !isStarted)
        {
            StartSpawners();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void CheckAvailable()
    {
        if (ghostsAlive >= maxGhosts)
        {
            print("Max ghosts reached");
            for (int i = 0; i < ghostSpawners.Length; i++)
            {
                if (ghostSpawners[i].currentGhost != null)
                {
                    //print("Ghost spawned on spawner" + ghostSpawners[i].transform.name);
                }
            }
            return;
        }
        else
        {
            for (int i = 0; i < ghostSpawners.Length; i++)
            {
                //print("current Ghost is " + ghostSpawners[i].currentGhost + " at spawner " + ghostSpawners[i].transform.name);
                if (ghostSpawners[i].currentGhost == null && !availableSpawners.Contains(ghostSpawners[i]))
                {
                    availableSpawners.Add(ghostSpawners[i]);
                }
            }
        }

        StartCoroutine(BigTimer());
    }
    [ServerRpc(RequireOwnership = false)]
    void StartSpawners()
    {
        isStarted = true;
        CheckAvailable();
    }
    [ServerRpc(RequireOwnership = false)]
    void PickSpawner(int spawnerCount)
    {
        if (ghostsAlive >= maxGhosts)
        {
            print("Max ghosts reached");
            return;
        }
        int i = Random.Range(0, spawnerCount);
        //print(spawnerCount + " availeble spawners. Spawner picked: " + i);
        availableSpawners[i].PickGhost();
        availableSpawners.Remove(availableSpawners[i]);
        CheckAvailable();
    }
    private void Update()
    {
        for (int i = 0; i < availableSpawners.Count - 1; i++)
        {
            if (ghostSpawners[i].currentGhost != null)
            {
                availableSpawners.Remove(ghostSpawners[i]);
            }
        }
    }
    IEnumerator BigTimer()
    {
        yield return new WaitForSeconds(spawnDelay);
        PickSpawner(availableSpawners.Count - 1);
    }
    public void ChangeGhostAlive(int amount)
    {
        ghostsAlive += amount;
    }
} // niggers