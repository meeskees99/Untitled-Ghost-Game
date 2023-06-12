using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class StofZuiger : NetworkBehaviour
{
    //[SerializeField] GameObject zuigBox;
    //[SerializeField] float suckRange;

    [SerializeField] KeyCode suck;
    [SerializeField] KeyCode shoot;
    [SerializeField] LayerMask mask;
    [SerializeField] PlayerData pData;
    [SerializeField] Transform shootPos;
    [SerializeField] float suckRange;

    [SerializeField] GameObject ghost;
    RaycastHit hit1;
    RaycastHit hit2;
    RaycastHit hit3;
    float time;

    [SerializeField] Animator animator;

    [SerializeField] StofZuiger[] stofZuigers;
    GameManager gameManager;

    [SerializeField] int maxGhostPoints = 3;
    [SyncVar] int ghostPoints;
    bool maxGhost;

    [SerializeField] List<GameObject> target = new();
    GameObject Kaas;
    string GhostTag = "Ghost";

    void Start()
    {

    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            GetComponent<MeshCollider>().enabled = false;
            this.enabled = false;
        }
        stofZuigers = FindObjectsOfType<StofZuiger>();
    }
    void Update()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        if (ghostPoints > maxGhostPoints)
        {
            int g = ghostPoints - maxGhostPoints;

            for (int x = 0; x < g; x++)
            {
                // Shoot excess ghost with shoot function
                //Shoot();
                ghostPoints--;
            }
            if (ghostPoints == maxGhostPoints)
            {
                maxGhost = true;
            }

        }
        else
        {
            maxGhost = false;
        }

        if (Input.GetKey(suck))
        {
            animator.SetBool("IsSucking", true);
            if (maxGhost || target == null)
                return;
            Suck();
        }
        else if (Input.GetKeyUp(suck))
        {
            animator.SetBool("IsSucking", false);
            for (int i = 0; i < target.Count; i++)
            {
                target[i].transform.GetComponent<GhostMovement>().isHit(false);
            }
        }
        else if (Input.GetKeyDown(shoot))
        {
            animator.SetTrigger("IsShooting");
            Shoot();
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
    public void Suck()
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target.Count >= 1)
            {
                Debug.DrawRay(shootPos.position, target[i].transform.position - shootPos.transform.position);
                if (Physics.Raycast(shootPos.position, target[i].transform.position - shootPos.position, out hit1, suckRange, mask))
                {
                    print("Nu shiet ik de raycast");
                    if (hit1.transform.tag == GhostTag)
                    {

                        if (hit1.transform.GetComponent<GhostMovement>().timeLeft() <= 0)
                        {
                            ghostPoints += target[i].transform.GetComponent<GhostMovement>().Points();
                            pData.GainPoints(target[i].transform.GetComponent<GhostMovement>().Points());

                            // error when removing for other players
                            target.Remove(target[i]);
                            
                            hit1.transform.GetComponent<GhostMovement>().Die();
                            print("Ghost " + i + " Cought");
                        }
                        else if (hit1.transform.GetComponent<GhostMovement>().timeLeft() >= 0)
                            hit1.transform.GetComponent<GhostMovement>().isHit(true);
                    }
                    else
                    {
                        target[i].transform.GetComponent<GhostMovement>().isHit(false);
                    }
                }
            }
            else
            {
                print("No Targets In Range!");
            }
        }
    }
    public void Shoot()
    {
        // GameObject spawnGhost = Instantiate(ghost);
        // Spawn(spawnGhost);
    }

    public void StorePoints()
    {
        gameManager.AddPoints(pData.teamID, ghostPoints);
        ghostPoints = 0;
    }
}
