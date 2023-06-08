using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class TestSpawner : NetworkBehaviour
{
    public Transform[] Spawnpoints;
    public GameObject ghost;
    GameObject homo;
    // Start is called before the first frame update
    void Start()
    {
        Spawning();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Spawning();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void Spawning()
    {
        for (int i = 0; i < Spawnpoints.Length; i++)
        {
            homo = Instantiate(ghost, Spawnpoints[i].position, Spawnpoints[i].rotation);
            Spawn(homo);
        }
    }
}
