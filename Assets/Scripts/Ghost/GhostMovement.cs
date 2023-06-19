using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using FishNet.Object;
using FishNet.Object.Synchronizing;
public class GhostMovement : NetworkBehaviour
{
    NavMeshAgent agent;

    [SerializeField] Animator animator;
    [SerializeField] GhostData ghostData;
    [Header("Walk Options")]
    [Tooltip("The layer the agent can walk on")]
    [SerializeField] LayerMask walkableLayer;
    [Tooltip("Max distance the agent can walk")]
    [SerializeField] float walkRadius;
    [SyncVar] float speed;
    [Tooltip("The time an agent will wait before going to it's next point")]
    [SerializeField] float waitTime;

    [Header("Ghost Options")]
    public float timeToSuck;
    float rechargeRate;
    public float suckieTimer;
    int points;
    [SerializeField] float ghostStoppingDistance;
    float timer;
    GhostManager ghostManager;

    public StofZuiger[] stofzuigers;
    [SyncVar] public bool isDead;
    public bool hitness;
    public bool[] hits;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsHost)
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }
        agent = GetComponent<NavMeshAgent>();
        ghostManager = FindObjectOfType<GhostManager>();
        points = ghostData.points;
        speed = ghostData.speed;
        walkRadius = ghostData.walkradius;
        timeToSuck = ghostData.suckTime;
        rechargeRate = ghostData.rechargeRate;

        agent.autoBraking = false;
        agent.stoppingDistance = ghostStoppingDistance;
        suckieTimer = timeToSuck;
        PatrolToNextPoint();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        stofzuigers = FindObjectsOfType<StofZuiger>();
    }
    bool someoneSucks;
    // Update is called once per frame
    void Update()
    {
        //print(isDead);
        if (isDead)
            return;

        PatrolToNextPoint();

        for (int i = 0; i < stofzuigers.Length; i++)
        {
            hits[i] = stofzuigers[i].sucking;
            for (int x = 0; x < hits.Length; x++)
            {
                if (hits[x])
                {
                    SetSpeed(0);
                    someoneSucks = true;
                }
                if (x == hits.Length && !someoneSucks)
                {
                    SetSpeed(ghostData.speed);
                    someoneSucks = false;
                }
            }
        }


        if (hitness)
        {
            GetSucked();
            BoolAnim("IsSucked", true);
        }
        else
        {
            SetSpeed(ghostData.speed);
            BoolAnim("IsSucked", false);
        }

        ResetSuckie();
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetSpeed(float speeds)
    {
        speed = speeds;
    }
    [ServerRpc(RequireOwnership = false)]
    void PatrolToNextPoint()
    {
        agent.speed = speed;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            BoolAnim("IsMoving", false);
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                BoolAnim("IsMoving", true);
                print("Going to next point");
                timer = waitTime;

                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                Vector3 finalPosition = hit.position;

                agent.destination = finalPosition;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void BoolAnim(string name, bool b)
    {
        animator.SetBool(name, b);
        AnimationObserver(name, b);
    }
    [ObserversRpc]
    public void AnimationObserver(string name, bool b)
    {
        if (IsHost)
            return;
        animator.SetBool(name, b);
    }
    public float timeLeft()
    {
        return suckieTimer;
    }
    
    public void isHit(bool hit)
    {
        hitness = hit;
    }

    public int Points()
    {
        return points;
    }

    void GetSucked()
    {
        print("Getting Head");
        suckieTimer -= Time.deltaTime;
    }
    [ServerRpc(RequireOwnership = false)]
    public void Die()
    {
        isDead = true;
        ghostManager.globalGhostPoints -= points;
        ghostManager.ChangeGhostAlive(-1);
        //InvokeRepeating("KillGhost", 0.1f, 0.1f;
        print("Ghost Dead");
        this.NetworkObject.Despawn();
    }
    void ResetSuckie()
    {
        if (!hitness && suckieTimer < timeToSuck)
        {
            suckieTimer += rechargeRate * Time.deltaTime;

        }
        else if (!hitness && suckieTimer > timeToSuck)
        {
            suckieTimer = timeToSuck;
        }
    }
    public int GetGhostValue()
    {
        return points;
    }
}
