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
    [SerializeField] int ghostPoints;
    bool maxGhost;

    [Tooltip("Set the fire rate to the amount of seconds you want to wait between shots")]
    [SerializeField] float fireRate;
    [SerializeField] float fireTime;

    [SerializeField] List<GameObject> target = new();
    string GhostTag = "Ghost";

    [SyncVar] public bool sucking;

    [SerializeField] GameObject beamparticlePrefab;

    [SerializeField] Transform beamstart;

    [SerializeField] List<GameObject> beams = new();
    [SerializeField] List<bool> beamSpawned = new();

    int oldTargetCount;

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
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        for (int z = 0; z < target.Count; z++)
        {
            if (target[z].transform.GetComponent<GhostMovement>().isDead)
            {
                target.Remove(target[z]);
            }
        }
        if (oldTargetCount != target.Count)
        {
            oldTargetCount = target.Count;
            beamSpawned.Clear();
            for (int i = 0; i < target.Count; i++)
            {
                beamSpawned.Add(false);
            }
        }

        if (ghostPoints > maxGhostPoints)
        {
            if (!maxGhost)
            {
                int g = (ghostPoints - maxGhostPoints);
                for (int x = 0; x < g; x++)
                {
                    // Shoot excess ghost with shoot function
                    Shoot(false);
                }
                if (ghostPoints == maxGhostPoints)
                {
                    maxGhost = true;
                }
            }
        }
        else if (ghostPoints == maxGhostPoints)
        {
            maxGhost = true;
        }
        else if (ghostPoints < maxGhostPoints)
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
        else
        {
            for (int x = 0; x < target.Count; x++)
            {
                target[x].transform.GetComponent<GhostMovement>().isHit(false);
            }
            StopSuck();
            for (int i = 0; i < beams.Count - 1; i++)
            {
                BeamDespawn(i);
            }
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
                    if (hit.transform.tag == GhostTag && !hit.transform.GetComponent<GhostMovement>().isDead)
                    {
                        if (target[i].transform.GetComponent<GhostMovement>().timeLeft() <= 0)
                        {
                            ghostPoints += target[i].transform.GetComponent<GhostMovement>().Points();
                            print(target[i].transform.GetComponent<GhostMovement>().Points());

                            target[i].transform.GetComponent<GhostMovement>().Die();
                            target.Remove(target[i]);
                            //BeamDespawn(i);
                            //beamSpawned[i] = false;
                        }
                        else if (target[i].transform.GetComponent<GhostMovement>().timeLeft() > 0)
                        {
                            target[i].transform.GetComponent<GhostMovement>().isHit(true);
                            SetSucking(true);

                            if (!beamSpawned[i])
                            {
                                Beamspawn(i);
                                beamSpawned[i] = true;
                            }
                            else
                            {
                                print("All beams spawned");
                            }

                            if (beams.Count - 1 == i)
                            {
                                if (beams[i] != null)
                                {
                                    beams[i].transform.GetChild(1).transform.position = beamstart.position;
                                    beams[i].transform.GetChild(2).transform.position = target[i].transform.position;
                                }
                            }

                        }
                    }
                }
            }
            else
            {
                beamSpawned.Clear();
                print("clear");
            }
        }
    }
    [ServerRpc(RequireOwnership = true)]
    public void Beamspawn(int i)
    {
        beams.Add(Instantiate(beamparticlePrefab));
        Spawn(beams[i]);

    }
    [ServerRpc(RequireOwnership = false)]
    public void BeamDespawn(int i)
    {
        GameObject gabagool = beams[i];
        beams.Remove(beams[i]);
        Despawn(gabagool);
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetSucking(bool state)
    {
        sucking = state;
    }
    [ServerRpc(RequireOwnership = true)]
    public void StopSuck()
    {
        SuckAnimation(false);
        SetSucking(false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GhostTag))
        {
            other.transform.GetComponent<GhostMovement>().isHit(false);
            SetSucking(false);
            //print("SetSucking = Fasle");

            //print(other + " removed");
            for (int i = 0; i < target.Count - 1; i++)
            {
                if (target[i] == other.gameObject && beams.Count != 0)
                {
                    BeamDespawn(i);
                }
            }
            target.Remove(other.gameObject);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GhostTag))
        {
            if (target.Contains(other.gameObject))
            {
                //print("alredy inig lsit");
                return;
            }
            //print(other + " Added");
            target.Add(other.gameObject);
        }
    }

    public void Shoot(bool isBullet)
    {
        ReleaseGhost(isBullet);
        ghostPoints -= 1;
    }

    [ServerRpc(RequireOwnership = true)]
    public void ReleaseGhost(bool isBullet)
    {
        GameObject spawnedBullet = Instantiate(playerBullet, shootPos.position, shootPos.rotation);
        Spawn(spawnedBullet, this.Owner);

        spawnedBullet.GetComponent<Rigidbody>().velocity = shootPos.forward * fireSpeed;
        spawnedBullet.GetComponent<Bullet>().isBullet = isBullet;
    }

    public void StorePoints()
    {
        pData.GainPoints(ghostPoints);
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
