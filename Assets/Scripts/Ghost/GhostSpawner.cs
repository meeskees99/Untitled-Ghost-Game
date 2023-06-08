using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class GhostSpawner : NetworkBehaviour
{
    [SerializeField] GameObject[] ghosts;
    public int maxIndex;
    [SerializeField] GameObject CurrentGhost;

    [SerializeField] int ghostOneSpawnThreshold;
    [SerializeField] int ghostTwoSpawnThreshold;
    [SerializeField] int ghostThreeSpawnThreshold;

    [SerializeField] int currentAvailablePoints;
    // Start is called before the first frame update
    void Start()
    {
        SpawnGhost();
        StartCoroutine("Spawner");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnGhost()
    {
        if (CurrentGhost != null)
            return;
        maxIndex = 2;
        if(currentAvailablePoints > ghostThreeSpawnThreshold)
            maxIndex = 1;
        else if(currentAvailablePoints > ghostTwoSpawnThreshold)
                maxIndex = 0;
        else if(currentAvailablePoints > ghostOneSpawnThreshold){
            print("Reached Max Ghost Points Amount Of " + currentAvailablePoints);
            return; 
        }
        
        int i = Random.Range(0 , maxIndex);
        currentAvailablePoints += ghosts[i].GetComponent<GhostMovement>().Points();
        CurrentGhost = Instantiate(ghosts[i]);
        Spawn(CurrentGhost);
    }
}