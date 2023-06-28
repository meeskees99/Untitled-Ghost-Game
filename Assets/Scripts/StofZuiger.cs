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
    [SerializeField] MovementAdvanced movement;
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
    public int GhostPoints
    {
        get
        {
            return ghostPoints;
        }
    }
    bool maxGhost;

    [Tooltip("Set the fire rate to the amount of seconds you want to wait between shots")]
    [SerializeField] float fireRate;
    [SerializeField] float fireTime;

    [SerializeField] List<GameObject> target = new();
    string GhostTag = "Ghost";

    [SyncVar] public bool sucking;

    [SerializeField] GameObject beamparticlePrefab;

    [SerializeField] Transform beamstart;

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
        if (animator == null)
        {
            return;
        }
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
                            AddPoints(target[i].transform.GetComponent<GhostMovement>().Points());
                            print(target[i].transform.GetComponent<GhostMovement>().Points());

                            target[i].transform.GetComponent<GhostMovement>().Die();
                            target.Remove(target[i]);
                        }
                        else if (target[i].transform.GetComponent<GhostMovement>().timeLeft() > 0)
                        {
                            target[i].transform.GetComponent<GhostMovement>().isHit(true);
                            SetSucking(true);
                        }
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    void AddPoints(int points)
    {
        ghostPoints += points;
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

    [ServerRpc(RequireOwnership = false)]
    public void StealPoints(int points, StofZuiger enemy)
    {
        int newPoints = (ghostPoints + points);
        int pointsToGain;
        if (newPoints > maxGhostPoints)
        {
            pointsToGain = newPoints - maxGhostPoints;
        }
        else
        {
            pointsToGain = points;
        }
        ghostPoints += pointsToGain;
        enemy.LosePoints(pointsToGain);
    }
    [ServerRpc(RequireOwnership = false)]
    public void LosePoints(int points)
    {
        ghostPoints -= points;
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
    [ServerRpc(RequireOwnership = true)]
    public void Shoot(bool isBullet)
    {
        ReleaseGhost(isBullet, this.NetworkObject, fireSpeed);
        ghostPoints -= 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReleaseGhost(bool isBullet, NetworkObject netObj, float speed)
    {
        GameObject spawnedBullet = Instantiate(playerBullet, shootPos.position, shootPos.rotation);
        print(netObj.OwnerId);
        Spawn(spawnedBullet, netObj.Owner);
        spawnedBullet.GetComponent<Bullet>().ownerofObject = this.NetworkObject;

        spawnedBullet.GetComponent<Rigidbody>().velocity = shootPos.forward * speed;
        spawnedBullet.GetComponent<Bullet>().isBullet = isBullet;

        UpdatePos(spawnedBullet, isBullet, speed);
    }
    [ObserversRpc]
    void UpdatePos(GameObject spawnedBullet, bool isBullet, float speed)
    {
        spawnedBullet.GetComponent<Rigidbody>().velocity = shootPos.forward * speed;
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
