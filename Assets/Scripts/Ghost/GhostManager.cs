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
    [SerializeField] int globalGhostPoints;

    [SerializeField] List<GhostSpawner> availableSpawners = new();

    [SerializeField] float spawnDelay = 1f;
    [SyncVar][SerializeField] bool isStarted;
    bool isSpawning;
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
            print("Max ghosts reached in CheckAvailable");
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
        isSpawning = true;
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
            print("Max ghosts reached in Pickspawner");
            return;
        }

        int i = Random.Range(0, spawnerCount);
        availableSpawners[i].PickGhost();
        availableSpawners.Remove(availableSpawners[i]);
        CheckAvailable();
    }
    private void Update()
    {
        CheckSpawners();
        if (ghostsAlive == maxGhosts)
        {
            isSpawning = false;
        }

        // finish later 
        if (!isSpawning && ghostsAlive < maxGhosts)
        {
            StartCoroutine(CheckTime());
            isSpawning = true;
        }
    }

    IEnumerator CheckTime()
    {
        yield return new WaitForSeconds(spawnDelay);
        CheckAvailable();
    }
    [ServerRpc(RequireOwnership = false)]
    void CheckSpawners()
    {
        for (int i = 0; i < availableSpawners.Count; i++)
        {
            if (availableSpawners[i].currentGhost != null)
            {
                // print("removed spawner + " + i);
                availableSpawners.Remove(availableSpawners[i]);
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
    public void ChangeGhostPoint(int amount)
    {
        globalGhostPoints += amount;
    }
}