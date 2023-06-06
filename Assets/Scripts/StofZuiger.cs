using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StofZuiger : MonoBehaviour
{
    //[SerializeField] GameObject zuigBox;
    //[SerializeField] float suckRange;
    [SerializeField] LayerMask mask;

    [SerializeField] Transform shootPos;
    RaycastHit hit1;
    RaycastHit hit2;
    RaycastHit hit3;
    float time;
    [SerializeField] List<GameObject> target = new();
    string GhostTag = "Ghost";

    void Update()
    {
        if (target == null)
        {
            return;
        }

        if (target.Count >= 1 && Physics.Raycast(shootPos.position, target[0].transform.position -= transform.position, out hit1, mask))
        {
            if (hit1.transform.tag == GhostTag)
            {
                if(hit1.transform.GetComponent<GhostMovement>().timeleft() <= 0){
                    print("Ghost Cought");
                }
                else if (hit1.transform.GetComponent<GhostMovement>().timeleft() > 0)
                    hit1.transform.GetComponent<GhostMovement>().isHit = true;
            }
            else
            {
                hit1.transform.GetComponent<GhostMovement>().isHit = false;
            }
        }
        if (target.Count >= 2 && Physics.Raycast(shootPos.position, target[1].transform.position -= transform.position, out hit2, mask))
        {
            if (hit2.transform.tag == GhostTag)
            {
                if (hit2.transform.GetComponent<GhostMovement>().timeleft() <= 0)
                    hit2.transform.GetComponent<GhostMovement>().isHit = true;
            }
            else
            {
                hit2.transform.GetComponent<GhostMovement>().isHit = false;
            }
        }
        if (target.Count >= 3 && Physics.Raycast(shootPos.position, target[2].transform.position -= transform.position, out hit3, mask))
        {
            if (hit3.transform.tag == GhostTag)
            {
                if (hit3.transform.GetComponent<GhostMovement>().timeleft() <= 0)
                    hit3.transform.GetComponent<GhostMovement>().isHit = true;
            }
            else
            {
                hit3.transform.GetComponent<GhostMovement>().isHit = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GhostTag))
        {
            print(other + " exit");
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
}
