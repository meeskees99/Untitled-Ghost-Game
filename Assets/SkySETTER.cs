using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySETTER : NetworkBehaviour
{
    [SerializeField] Material _sky;
    public override void OnStartClient()
    {
        base.OnStartClient();
        RenderSettings.skybox = _sky;
    }
}
