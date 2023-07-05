using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class Bullet : NetworkBehaviour
{
    [SerializeField] GameObject ghost;

    [SerializeField] float bulletLifeTime = 3f;

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
        other.transform.GetComponent<MovementAdvanced>().SetCanSteal(true);
        DoDespawn();
    }
    [ServerRpc(RequireOwnership = false)]
    public void DoDespawn()
    {
        Despawn(gameObject);
    }
    private void Update()
    {
        if (bulletLifeTime <= 0)
        {
            DoDespawn();
        }
        if (bulletLifeTime > 0)
        {
            bulletLifeTime -= Time.deltaTime;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void Check(GameObject other, NetworkObject netObj)
    {
        if (other == null || netObj == null)
        {
            return;
        }
        print(other.gameObject.name);

        if (other.transform.tag == "Player")
        {
            if (netObj.OwnerId == other.GetComponent<NetworkObject>().OwnerId)
            {
                print("Owner");
                return;
            }
            print(other.transform.GetComponent<PlayerData>().username);
            DoStun(other);
        }
        else if (other.transform.tag != "SuckBox")
        {
            DoDespawn();
        }

    }
}
