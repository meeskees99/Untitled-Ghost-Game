using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class Bullet : NetworkBehaviour
{
    [SerializeField] GameObject ghost;

    public bool isBullet;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<NetworkObject>().IsOwner)
        {
            print("Owner");
            return;
        }
        if (isBullet)
        {
            if (other.transform.tag == "Player")
            {
                other.transform.GetComponent<MovementAdvanced>().Stun();
                Despawn(gameObject);
            }
            else
            {
                Despawn(gameObject);
            }
        }
        else 
        {
            GameObject CGhost = Instantiate(ghost);
            Spawn(CGhost);
        }
    }
}
