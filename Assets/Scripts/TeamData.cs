using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamData : NetworkBehaviour
{
    [SyncVar]
    public List<PlayerData> tData = new List<PlayerData>();
}
