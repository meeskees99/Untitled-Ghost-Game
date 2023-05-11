using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class GhostMovement : MonoBehaviour
{
    NavMeshAgent agent;
    RaycastHit hit;
    [SerializeField] Camera cam;
    [Header("Settings")]
    [Tooltip("The layer the agent can walk on")]
    [SerializeField]LayerMask walkableLayer;
    [Tooltip("Put an empty GameObject here with the desired patrol potitions as children")]
    [SerializeField] Transform target;
    [Tooltip("The time an agent will wait before going to it's next point")]
    [SerializeField] float waitTime;
    [SerializeField] TMP_Text timerTxt;
    float timer;
    Transform[] targets;
    bool toggle;
    int targetIndex;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.autoBraking = false;
              
        targets = new Transform[target.childCount];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = target.GetChild(i);
        }

        timer = waitTime;
        PatrolToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {      
        //Toggle Patrol
        if (Input.GetButtonDown("Jump"))
        {
            toggle = !toggle;
            print("Toggled Patrol to " + toggle);
        }
        if (!toggle)
        {
            timerTxt.text = "Paused patroling.\nPress 'Space' to start";
            return;
        }
        if(!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            timer -= Time.deltaTime;
            timerTxt.text = "Time to next patrol: " + timer.ToString("0.##");
            if (timer <= 0)
            {
                print("Going to next point");
                PatrolToNextPoint();
            }
            
        }
        // Pick a spot with mouse
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, walkableLayer))
            {
                MoveAgentOnClick();
            }
        }
        
    }

    void PatrolToNextPoint()
    {
        timer = waitTime;
        timerTxt.text = "Patroling...";
        if (!toggle)
            return;
        if(targets.Length == 0)
        {
            Debug.LogError("No Targets Found! Set them up in the inspector.");
            return; 
        }

        agent.destination = targets[targetIndex].position;

        //Get next point in array, go back to start if at end
        targetIndex = (targetIndex + 1) % targets.Length;
    }

    void MoveAgentOnClick()
    {
        agent.destination = hit.point;
    }
}
