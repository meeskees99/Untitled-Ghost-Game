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
    RaycastHit hit;
    float time;

    [SerializeField] Animator animator;

    [SerializeField] StofZuiger[] stofZuigers;
    GameManager gameManager;

    [SerializeField] int maxGhostPoints = 3;
    [SyncVar][SerializeField] int ghostPoints;
    bool maxGhost;

    [SerializeField] List<GameObject> target = new();
    string GhostTag = "Ghost";
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
            if (maxGhost || target == null)
                return;
            Suck();
            SuckAnimation(true);
        }
        else if (!Input.GetKey(suck))
        {
            SuckAnimation(false);

            for (int i = 0; i < target.Count; i++)
            {
                target[i].transform.GetComponent<GhostMovement>().isHit(false);
            }
        }
        if (Input.GetKeyDown(shoot))
        {
            ShootAnimation();

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
                if (Physics.Raycast(shootPos.position, target[i].transform.position - shootPos.position, out hit, suckRange, mask))
                {
                    print("Nu shiet ik de raycast");
                    if (hit.transform.tag == GhostTag && !hit.transform.GetComponent<GhostMovement>().isDead)
                    {
                        if (hit.transform.GetComponent<GhostMovement>().timeLeft() <= 0)
                        {
                            ghostPoints += target[i].transform.GetComponent<GhostMovement>().Points();

                            // error when removing for other players
                            hit.transform.GetComponent<GhostMovement>().Die();
                            target.Remove(target[i]);
                        }
                        else if (hit.transform.GetComponent<GhostMovement>().timeLeft() >= 0)
                            hit.transform.GetComponent<GhostMovement>().isHit(true);
                    }
                    else if (target[i].transform.GetComponent<GhostMovement>().isDead)
                    {
                        target.Remove(target[i]);
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
    [ServerRpc(RequireOwnership = true)]
    public void ShootAnimation()
    {
        animator.SetTrigger("IsShooting");
        ShootAnimationObserver();
    }
    [ObserversRpc]
    public void ShootAnimationObserver()
    {
        if (IsHost)
            return;
        animator.SetTrigger("IsShooting");
    }
    [ServerRpc(RequireOwnership = true)]
    public void SuckAnimation(bool suckstate)
    {
        animator.SetBool("IsSucking", suckstate);
        SuckAnimationObserver(suckstate);
    }
    [ObserversRpc]
    public void SuckAnimationObserver(bool suckstate)
    {
        if (IsHost)
            return;
        animator.SetBool("IsSucking", suckstate);
    }
}
