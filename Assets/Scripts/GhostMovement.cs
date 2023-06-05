using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class GhostMovement : MonoBehaviour
{
    NavMeshAgent agent;

    [Header("Settings")]
    [Tooltip("The layer the agent can walk on")]
    [SerializeField]LayerMask walkableLayer;
    [Tooltip("Max distance the agent can walk")]
    [SerializeField] float walkRadius;
    [Tooltip("The time an agent will wait before going to it's next point")]
    [SerializeField] float waitTime;
    

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.autoBraking = false;
        
        timer = waitTime;
        PatrolToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {      
        if(!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                print("Going to next point");
                PatrolToNextPoint();
            }
            
        }
    }

    void PatrolToNextPoint()
    {
        timer = waitTime;

        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;

        agent.destination = finalPosition;
    }
}
