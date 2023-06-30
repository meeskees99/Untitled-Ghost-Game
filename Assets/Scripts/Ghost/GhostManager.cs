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

    List<GhostSpawner> availableSpawners = new();


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
        availableSpawners.Clear();
        for (int i = 0; i < ghostSpawners.Length; i++)
        {
            print("current Ghost is " + ghostSpawners[i].currentGhost + " at spawner " + ghostSpawners[i].transform.name);
            if (ghostSpawners[i].currentGhost == null)
            {
                availableSpawners.Add(ghostSpawners[i]);
            }
        }
        PickSpawner();
    }

    private void Update()
    {
        for (int i = 0; i < ghostSpawners.Length; i++)
        {
            print("current Ghost is " + ghostSpawners[i].currentGhost + " at spawner " + ghostSpawners[i].transform.name);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void StartSpawners()
    {
        isStarted = true;
        for (int i = 0; i < maxGhosts; i++)
        {
            CheckAvailable();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void PickSpawner()
    {
        int i = Random.Range(0, availableSpawners.Count - 1);
        availableSpawners[i].PickGhost();
    }
    public void ChangeGhostAlive(int amount)
    {
        ghostsAlive += amount;
    }
}
