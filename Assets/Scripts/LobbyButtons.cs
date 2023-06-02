using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButtons : NetworkBehaviour
{
    NetworkHudCanvases networkHudCanvases;
    // Start is called before the first frame update
    void Start()
    {
        networkHudCanvases = FindObjectOfType<NetworkHudCanvases>();
    }

    public void ClickClient()
    {
        if (IsHost)
        {
            ClickHost();
        }
        else
        {
            networkHudCanvases.OnClick_Client();
        }
    }

    public void ClickHost()
    {
        networkHudCanvases.OnClick_Server();
        networkHudCanvases.OnClick_Client();
    }
}
