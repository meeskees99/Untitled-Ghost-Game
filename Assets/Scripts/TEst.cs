using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEst : NetworkBehaviour
{
    
    void Start()
    {
        TESTSERVER();
    }

    [ServerRpc(RequireOwnership = false)] public void TESTSERVER()
    {
        print("server");
        TESTOBSERVER();
    }
    [ObserversRpc(BufferLast = true)] public void TESTOBSERVER()
    {
        print("observer");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            TESTSERVER();
        }
    }
}
