using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Bullet : NetworkBehaviour
{
    [SerializeField] GameObject ghost;

    [SerializeField] bool isBullet;
    [SerializeField] bool isGhost;
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
        if (isBullet)
        {
            print(" bullet");
            if (other.transform.tag == "Player")
            {
                other.transform.GetComponent<MovementAdvanced>().Stun();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (isGhost)
        {
            GameObject CGhost =Instantiate(ghost);
            Spawn(CGhost);
        }
        else
        {
            print("Please Set The Bullet To Either 'isBullet' or 'isGhost'.");
        }

    }
}
