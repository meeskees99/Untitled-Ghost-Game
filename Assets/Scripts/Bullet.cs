using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class Bullet : NetworkBehaviour
{
    [SerializeField] GameObject ghost;

    public bool isBullet;

    [SyncVar] public NetworkObject ownerofObject;
    private void OnTriggerEnter(Collider other)
    {
        print(ownerofObject.OwnerId + " shooter");
        Check(other.gameObject, ownerofObject);
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
        if (other.transform.tag == "Player")
        {
            if (netObj.OwnerId == other.GetComponent<NetworkObject>().OwnerId)
            {
                print("Owner");
                return;
            }
            if (isBullet)
            {
                print(other.transform.GetComponent<PlayerData>().username);
                DoStun(other.gameObject);
            }
            else
            {
                GameObject CGhost = Instantiate(ghost);
                Spawn(CGhost);
            }
        }
        else if (other.transform.tag != "SuckBox")
        {
            DoDespawn();
        }
    }
}
