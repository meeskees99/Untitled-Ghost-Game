using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamData : NetworkBehaviour
{
    [SyncObject] public readonly SyncList<PlayerData> tData = new SyncList<PlayerData>();
}
