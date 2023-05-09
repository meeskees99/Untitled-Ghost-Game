using FishNet.Transporting.Tugboat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPscript : MonoBehaviour
{
    [SerializeField] private Tugboat boat;
    public void ChangeIpClient(string ip)
    {
        boat.SetClientAddress(ip);
    }
    public void ChangeIpServer(string ip)
    {
        boat.SetServerBindAddress(ip, FishNet.Transporting.IPAddressType.IPv4);
    }
}
