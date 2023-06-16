using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class Bullet : NetworkBehaviour
{
    [SerializeField] GameObject ghost;

    public bool isBullet;

    private void OnCollisionEnter(Collision other)
    {
        Check(other.gameObject);
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
    public void Check(GameObject other)
    {
        print(other.transform.GetComponent<PlayerData>().username);
        if (other.transform.GetComponent<NetworkObject>().IsOwner)
        {
            print("Owner");
            return;
        }
        if (isBullet)
        {
            if (other.transform.tag == "Player" && !other.transform.GetComponent<NetworkObject>().IsOwner)
            {
                print(other.transform.GetComponent<PlayerData>().username);
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
