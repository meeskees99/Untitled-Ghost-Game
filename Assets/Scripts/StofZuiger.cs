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

    // [SerializeField] List<GameObject> beams = new();
    // [SerializeField] List<bool> beamSpawned = new();

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
        // if (oldTargetCount != target.Count)
        // {
        //     oldTargetCount = target.Count;
        //     beamSpawned.Clear();
        //     for (int i = 0; i < target.Count; i++)
        //     {
        //         beamSpawned.Add(false);
        //     }
        // }

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
        // if (target.Count == 0)
        // {
        //     for (int i = 0; i < beams.Count; i++)
        //     {
        //         canHandle[i] = false;
        //         StartCoroutine(WaitForDeSpawn(beams[i], i));
        //         beams.Remove(beams[i]);
        //     }
        // }
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
            // if (beams.Count > 0)
            // {
            //     canHandle[0] = false;
            //     StartCoroutine(WaitForDeSpawn(beams[0], 0));
            //     if (beamSpawned.Count > 0)
            //     {
            //         for (int i = 0; i < beamSpawned.Count; i++)
            //         {
            //             beamSpawned[i] = false;
            //         }
            //     }
            // }
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
                            //canHandle[i] = false;
                            //StartCoroutine(WaitForDeSpawn(beams[i], i));
                            //beamSpawned[i] = false;
                        }
                        else if (target[i].transform.GetComponent<GhostMovement>().timeLeft() > 0)
                        {
                            target[i].transform.GetComponent<GhostMovement>().isHit(true);
                            SetSucking(true);
                            // if (!beamSpawned[i] && beams.Count < target.Count)
                            // {
                            //     Beamspawn(i);
                            //     beamSpawned[i] = true;
                            //     canHandle.Add(true);
                            // }
                            // else
                            // {
                            //     print("All beams spawned");
                            // }

                            // if (beams.Count - 1 <= i)
                            // {
                            //     print("pos");
                            //     print(i);
                            //     HandleBeamPos(i, target[i]);
                            // }
                        }
                    }
                }
            }
            // else
            // {
            //     beamSpawned.Clear();
            // }
        }
    }
    // [ServerRpc(RequireOwnership = true)]
    // public void HandleBeamPos(int i, GameObject targets)
    // {
    //     if (beams.Count - 1 <= i && canHandle[i])
    //     {
    //         beams[i].transform.GetChild(1).transform.position = beamstart.position;

    //         beams[i].transform.GetChild(2).transform.position = targets.transform.position;
    //     }
    // }
    // [SerializeField] List<bool> canHandle = new();
    // [ServerRpc(RequireOwnership = true)]
    // public void Beamspawn(int i)
    // {
    //     beams.Add(Instantiate(beamparticlePrefab));
    //     Spawn(beams[i]);

    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void BeamDespawn(GameObject beam, bool i)
    // {
    //     GameObject gabagool = beam;
    //     beams.Remove(beam);
    //     Despawn(gabagool);
    //     canHandle.Remove(i);
    // }

    // public IEnumerator WaitForDeSpawn(GameObject beam, int i)
    // {
    //     yield return new WaitForSeconds(0.1f);
    //     print("despawnWait");
    //     BeamDespawn(beam, canHandle[i]);
    // }

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
            target.Remove(other.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GhostTag))
        {
            if (target.Contains(other.gameObject))
            {
                return;
            }
            target.Add(other.gameObject);
        }
    }

    public void Shoot(bool isBullet)
    {
        ReleaseGhost(isBullet, this.NetworkObject);
        ghostPoints -= 1;
    }

    [ServerRpc(RequireOwnership = true)]
    public void ReleaseGhost(bool isBullet, NetworkObject netObj)
    {
        GameObject spawnedBullet = Instantiate(playerBullet, shootPos.position, shootPos.rotation);
        print(netObj.OwnerId);
        Spawn(spawnedBullet, netObj.Owner);

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
