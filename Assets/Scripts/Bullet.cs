using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class Bullet : NetworkBehaviour
{
    [SerializeField] GameObject ghost;

    public bool isBullet;

    public NetworkObject ownerofObject;
    private void OnTriggerEnter(Collider other)
    {
        Check(other.gameObject, ownerofObject);
        print(NetworkObject.OwnerId + " Collision");
        print(ownerofObject.OwnerId + " shooter");
    }

    [ServerRpc(RequireOwnership = false)]
    public void DoStun(GameObject other)
    {
        other.transform.GetComponent<MovementAdvanced>().Stun();
        DoDespawn();
    }
    [ServerRpc(RequireOwnership = false)]
    public void DoDespawn()
    {
        Despawn(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void Check(GameObject other, NetworkObject netObj)
    {
        print(other.transform.GetComponent<PlayerData>().username);
        print(netObj.Owner);
        print(this.NetworkObject.Owner);
        if (netObj.Owner == this.NetworkObject.Owner)
        {
            print("Owner");
            return;
        }
        if (isBullet)
        {
            if (other.transform.tag == "Player" && !other.transform.GetComponent<NetworkObject>().IsOwner)
            {
                //print(other.transform.GetComponent<PlayerData>().username);
                DoStun(other.gameObject);
            }
            else
            {
                DoDespawn();
            }
        }
        else
        {
            GameObject CGhost = Instantiate(ghost);
            Spawn(CGhost);
        }
    }
}
