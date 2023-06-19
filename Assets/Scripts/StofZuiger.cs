using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class StofZuiger : NetworkBehaviour
{
    //[SerializeField] GameObject zuigBox;
    //[SerializeField] float suckRange;
    [Header("Setup")]
    [SerializeField] KeyCode suck;
    [SerializeField] KeyCode shoot;
    [SerializeField] LayerMask mask;
    [SerializeField] PlayerData pData;
    [Header("Shooting")]
    [SerializeField] Transform shootPos;
    [SerializeField] float suckRange;
    [SerializeField] float fireSpeed;
    [SerializeField] GameObject ghostToShoot;
    [SerializeField] GameObject playerBullet;

    RaycastHit hit;
    float time;

    [SerializeField] Animator animator;
    GameManager gameManager;

    [SerializeField] int maxGhostPoints = 3;
    [SyncVar][SerializeField] int ghostPoints;
    bool maxGhost;

    [Tooltip("Set the fire rate to the amount of seconds you want to wait between shots")]
    [SerializeField] float fireRate;
    [SerializeField] float fireTime;

    [SerializeField] List<GameObject> target = new();
    string GhostTag = "Ghost";

    [SyncVar] public bool sucking;

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
        for (int z = 0; z < target.Count; z++)
        {
            if (target[z].transform.GetComponent<GhostMovement>().isDead)
            {
                target.Remove(target[z]);
            }
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        if (ghostPoints > maxGhostPoints)
        {
            if (!maxGhost)
            {
                int g = ghostPoints - maxGhostPoints;
                print(g + " g");
                for (int x = 0; x < g; x++)
                {
                    // Shoot excess ghost with shoot function
                    print(x + " x");
                    Shoot(false);
                }
                if (ghostPoints == maxGhostPoints)
                {
                    maxGhost = true;
                }
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
            StopSuck();
        }
        if (fireTime > 0)
        {
            fireTime -= Time.deltaTime;
        }
        if (Input.GetKeyDown(shoot) && ghostPoints > 0)
        {
            if (fireTime <= 0)
            {
                fireTime = fireRate;
                ShootAnimation();
                Shoot(true);

            }
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
                        print("Heeft ghosttag en is niet dood");
                        if (hit.transform.GetComponent<GhostMovement>().timeLeft() <= 0)
                        {
                            ghostPoints += target[i].transform.GetComponent<GhostMovement>().Points();

                            hit.transform.GetComponent<GhostMovement>().Die();
                            target.Remove(target[i]);
                        }
                        else if (hit.transform.GetComponent<GhostMovement>().timeLeft() >= 0)
                        {
                            print("Kanker hard aan t zuigen");
                            hit.transform.GetComponent<GhostMovement>().isHit(true);
                            SetSucking(true);
                        }
                    }
                }
            }
            else
            {
                print("No Targets In Range!");
            }
        }
    }
    [ServerRpc(RequireOwnership = true)]
    public void SetSucking(bool state)
    {
        sucking = state;
    }
    [ServerRpc(RequireOwnership = true)]
    public void StopSuck()
    {
        SuckAnimation(false);

        for (int i = 0; i < target.Count; i++)
        {
            target[i].transform.GetComponent<GhostMovement>().isHit(false);

            SetSucking(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GhostTag))
        {
            print(other + " exit");
            other.transform.GetComponent<GhostMovement>().isHit(false);
            SetSucking(false);

            target.Remove(other.gameObject);
            //RemoveTarget(other.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GhostTag))
        {
            print(other + " enter");
            target.Add(other.gameObject);
            //AddTarget(other.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = true)]
    public void AddTarget(GameObject newTarget)
    {
        target.Add(newTarget);
    }
    [ServerRpc(RequireOwnership = true)]
    public void RemoveTarget(GameObject oldTarget)
    {
        target.Remove(oldTarget);
    }

    [ServerRpc(RequireOwnership = true)]
    public void Shoot(bool isBullet)
    {
        ghostPoints--;
        print("shoot");
        GameObject spawnedBullet = Instantiate(playerBullet, shootPos.position, shootPos.rotation);
        Spawn(spawnedBullet, this.Owner);

        spawnedBullet.GetComponent<Rigidbody>().velocity = shootPos.forward * fireSpeed;
        spawnedBullet.GetComponent<Bullet>().isBullet = isBullet;
    }

    public void ReleaseGhost()
    {
        GameObject spawnGhost = Instantiate(ghostToShoot);
        Spawn(spawnGhost);
    }

    public void StorePoints()
    {
        gameManager.AddPoints(pData.teamID, ghostPoints);
        ghostPoints = 0;
    }

    #region Animations

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
    #endregion
}
