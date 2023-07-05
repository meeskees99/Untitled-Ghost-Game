using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButtons : MonoBehaviour
{
    NetworkHudCanvases networkHudCanvases;
    // Start is called before the first frame update
    void Start()
    {
        networkHudCanvases = FindObjectOfType<NetworkHudCanvases>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void ClickClient()
    {
        if (InstanceFinder.ClientManager.Connection.ClientId == 0)
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

    public void Quit()
    {
        Application.Quit();
    }
}
