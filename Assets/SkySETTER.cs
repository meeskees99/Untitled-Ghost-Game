using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySETTER : NetworkBehaviour
{
    public Material _sky;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    [ObserversRpc]
    public void SetSkybox()
    {
        RenderSettings.skybox = _sky;
        if (RenderSettings.skybox = _sky)
        {
            can = true;
        }
    }

    bool can;
    // Update is called once per frame
    void Update()
    {
        if (!can)
        {
            SetSkybox();
        }
    }
}
