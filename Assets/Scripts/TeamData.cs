using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamData : MonoBehaviour
{
    [SyncVar]
    public List<PlayerData> tData = new List<PlayerData>();
    public int teamIndex;
}
