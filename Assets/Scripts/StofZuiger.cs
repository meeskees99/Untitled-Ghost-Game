using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class StofZuiger : NetworkBehaviour
{
    //[SerializeField] GameObject zuigBox;
    //[SerializeField] float suckRange;
    [SerializeField] LayerMask mask;
    [SerializeField] PlayerData pData;
    [SerializeField] Transform shootPos;
    [SerializeField] float suckRange;
    RaycastHit hit1;
    RaycastHit hit2;
    RaycastHit hit3;
    float time;
    [SerializeField] List<GameObject> target = new();
    GameObject Kaas;
    string GhostTag = "Ghost";

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<MeshCollider>().enabled = false;
            this.enabled = false;
        }
    }
    void Update()
    {
        if (target == null)
        {
            return;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            #region Ghost 1
            if (target.Count >= 1)
            {
                Debug.DrawRay(shootPos.position, target[0].transform.position - shootPos.transform.position);
                if (Physics.Raycast(shootPos.position, target[0].transform.position - shootPos.position, out hit1, suckRange, mask))
                {
                    print("Nu shiet ik de raycast");
                    if (hit1.transform.tag == GhostTag)
                    {

                        if (hit1.transform.GetComponent<GhostMovement>().timeLeft() <= 0)
                        {
                            pData.GainPoints(target[0].transform.GetComponent<GhostMovement>().Points());
                            target.Remove(target[0]);
                            hit1.transform.GetComponent<GhostMovement>().Die();
                            print("Ghost 1 Cought");
                        }
                        else if (hit1.transform.GetComponent<GhostMovement>().timeLeft() >= 0)
                            hit1.transform.GetComponent<GhostMovement>().isHit(true);
                    }
                    else
                    {
                        target[0].transform.GetComponent<GhostMovement>().isHit(false);
                    }
                }


            }
            else
            {
                print("No Targets In Range!");
            }
            #endregion
            #region Ghost 2
            if (target.Count >= 2 && Physics.Raycast(shootPos.position, target[1].transform.position - shootPos.position, out hit2, suckRange, mask))
            {
                Debug.DrawRay(shootPos.position, target[1].transform.position - shootPos.transform.position);
                if (hit2.transform.tag == GhostTag)
                {
                    if (hit2.transform.GetComponent<GhostMovement>().timeLeft() <= 0)
                    {
                        pData.GainPoints(target[1].transform.GetComponent<GhostMovement>().Points());
                        target.Remove(target[1]);
                        hit2.transform.GetComponent<GhostMovement>().Die();
                        print("Ghost 2 Cought");
                    }
                    hit2.transform.GetComponent<GhostMovement>().isHit(true);
                }
                else
                {
                    target[1].transform.GetComponent<GhostMovement>().isHit(false);
                }
            }
            #endregion
            #region Ghost 3
            if (target.Count >= 3 && Physics.Raycast(shootPos.position, target[2].transform.position - shootPos.position, out hit3, suckRange, mask))
            {
                Debug.DrawRay(shootPos.position, target[2].transform.position - shootPos.transform.position);
                if (hit3.transform.tag == GhostTag)
                {
                    if (hit3.transform.GetComponent<GhostMovement>().timeLeft() <= 0)
                    {
                        pData.GainPoints(target[2].transform.GetComponent<GhostMovement>().Points());
                        target.Remove(target[2]);
                        hit3.transform.GetComponent<GhostMovement>().Die();
                        print("Ghost 3 Cought");
                    }
                    hit3.transform.GetComponent<GhostMovement>().isHit(true);
                }
                else
                {
                    target[2].transform.GetComponent<GhostMovement>().isHit(false);
                }
            }
            #endregion
        }
        else
        {
            for (int i = 0; i < target.Count; i++)
            {
                target[i].transform.GetComponent<GhostMovement>().isHit(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GhostTag))
        {
            print(other + " exit");
            other.transform.GetComponent<GhostMovement>().isHit(false);
            target.Remove(other.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GhostTag))
        {
            print(other + " enter");
            target.Add(other.gameObject);
        }
    }
    [SerializeField] bool doGizmos;
    [SerializeField] float sphereSize;
    private void OnDrawGizmos()
    {
        if (doGizmos)
        {
            Gizmos.DrawSphere(shootPos.position, sphereSize);
        }
    }
}
