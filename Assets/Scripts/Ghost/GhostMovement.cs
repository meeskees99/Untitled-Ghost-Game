using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using FishNet.Object;

public class GhostMovement : NetworkBehaviour
{
    NavMeshAgent agent;
    [SerializeField] GhostData ghostData;
    [Header("Walk Options")]
    [Tooltip("The layer the agent can walk on")]
    [SerializeField] LayerMask walkableLayer;
    [Tooltip("Max distance the agent can walk")]
    [SerializeField] float walkRadius;
    float speed;
    [Tooltip("The time an agent will wait before going to it's next point")]
    [SerializeField] float waitTime;

    [Header("Ghost Options")]
    public float timeToSuck;
    float rechargeRate;
    public float suckieTimer;
    int points;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        points = ghostData.points;
        speed = ghostData.speed;
        walkRadius = ghostData.walkradius;
        timeToSuck = ghostData.suckTime;
        rechargeRate = ghostData.rechargeRate;

        agent.autoBraking = false;
        suckieTimer = timeToSuck;
        PatrolToNextPoint();
        GetComponent<MeshRenderer>().enabled = true;

    }
    // Update is called once per frame
    void Update()
    {
        PatrolToNextPoint();

        if (hitness)
        {
            GetSucked();
        }

        ResetSuckie();
    }

    [ServerRpc(RequireOwnership = false)]
    void PatrolToNextPoint()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
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
    public float timeLeft()
    {
        return suckieTimer;
    }
    public bool hitness;
    public void isHit(bool hit)
    {
        hitness = hit;
    }

    public int Points(){
        return points;
    }

    void GetSucked()
    {
        suckieTimer -= Time.deltaTime;
    }

    public void Die()
    {
        print("Ghost Dead");
        Despawn(gameObject);
        Destroy(gameObject);
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
}
