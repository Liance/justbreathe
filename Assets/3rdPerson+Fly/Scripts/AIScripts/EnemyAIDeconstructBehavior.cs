using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAIDeconstructBehavior : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    public Transform target;

    public enum AIState { WANDERING, NAVIGATING, PUSHING }
    public GameObject[] wanderWaypoints;
    public int[] visitedWaypoints;
    public int currentWaypoint = 0;
    public AIState state = AIState.WANDERING;
    private Transform playerTransform;
    private LayerMask aiLayerMask;

    private Transform playerLastSeenTransform;

    private bool isWandering = false;
    private bool isNavigating = false;
    private bool isDestroying = false;

    public float viewingAngle = 60.0f;
    public float visibilityDist = 60.0f;

    private WaitForSeconds wanderRefreshDuration = new WaitForSeconds(1.0f);
    private WaitForSeconds navigateRefreshDuration = new WaitForSeconds(1.0f);
    private WaitForSeconds pushRefreshDuration = new WaitForSeconds(1.0f);

    private Vector3 dirToPlayer = new Vector3();
    private Vector3 dirToLastSeenPlayer = new Vector3();

    public float waypointMinReachDist = 5.0f;

    public Transform highestObservableStructure;
    public float groundHeight = 0.0f;
    public float sturctureHeightThreshold = 3.0f;

    public GameObject[] targetStructures;
    public GameObject[] targetStructuresAboveHeightThreshold;
    public int targetStructureID = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        wanderWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        visitedWaypoints = new int[wanderWaypoints.Length];
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        aiLayerMask = LayerMask.NameToLayer("Bot");
        SetNextWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        if (agent.remainingDistance < agent.stoppingDistance)
        {
            agent.Move(agent.desiredVelocity);
        }
        
        dirToPlayer = playerTransform.position - transform.position;
        switch (state)
        {
            case AIState.WANDERING:
                if (!isWandering)
                {
                    isWandering = true;
                    StartCoroutine(Wander());
                }
                break;
            case AIState.NAVIGATING:
                if (!isNavigating)
                {
                    isNavigating = true;
                    StartCoroutine(NavigateToStructure());
                }
                break;
            case AIState.PUSHING:
                if (!isDestroying)
                {
                    isDestroying = true;
                    StartCoroutine(PushStructure());
                }
                break;
        }
    }

    private void SetNextWaypoint()
    {
        int closestWaypointID = 0;
        float shortestDistToWaypoint = Mathf.Infinity;
        bool resetVisitedWaypoints = true;
        for (int i = 0; i < wanderWaypoints.Length; ++i)
        {
            float distToWaypoint = Vector3.Distance(wanderWaypoints[i].transform.position, transform.position);
            if (distToWaypoint < shortestDistToWaypoint && visitedWaypoints.All(w => w != i))
            {
                shortestDistToWaypoint = distToWaypoint;
                closestWaypointID = i;
                resetVisitedWaypoints = false;
            }
        }
        target = wanderWaypoints[closestWaypointID].transform;
        if (resetVisitedWaypoints)
        {
            visitedWaypoints = new int[wanderWaypoints.Length];
        }
        visitedWaypoints[closestWaypointID] = currentWaypoint = closestWaypointID;
    }

    private IEnumerator Wander()
    {
        while (isWandering)
        {
            if(Vector3.Distance(target.position, transform.position) <= waypointMinReachDist)
            {
                SetNextWaypoint();
            }
            targetStructures = GameObject.FindGameObjectsWithTag("Destructible");
            
            if (targetStructures.Any())
            {
                float shortestDistToPlayer = Mathf.Infinity;
                for (int i = 0; i < targetStructures.Length; ++i)
                {
                    float distToPlayer = Vector3.Distance(targetStructures[i].transform.position, playerTransform.position);
                    if (groundHeight + targetStructures[i].transform.position.y >= sturctureHeightThreshold && distToPlayer < shortestDistToPlayer)
                    {
                        shortestDistToPlayer = distToPlayer;
                        targetStructureID = i;
                    }
                }
                target = targetStructures[targetStructureID].transform;
                state = AIState.NAVIGATING;
                isWandering = false;
            }
            yield return wanderRefreshDuration;
        }
    }

    private IEnumerator NavigateToStructure()
    {
        while (isNavigating)
        {
            if(Vector3.Distance(target.position, transform.position) <= waypointMinReachDist)
            {
                state = AIState.PUSHING;
                isNavigating = false;
            }
            yield return navigateRefreshDuration;
        }
    }

    private IEnumerator PushStructure()
    {
        while (isDestroying)
        {
            transform.LookAt(target);
            target.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 100.0f, ForceMode.Impulse);
            //isDestroying = false;
            state = AIState.WANDERING;
            isDestroying = false;
            yield return pushRefreshDuration;
        }
    }
}
