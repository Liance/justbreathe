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
    private bool isPushing = false;

    public float viewingAngle = 60.0f;
    public float visibilityDist = 60.0f;

    private WaitForSeconds wanderRefreshDuration = new WaitForSeconds(1.0f);
    private WaitForSeconds navigateRefreshDuration = new WaitForSeconds(2.5f);
    private WaitForSeconds pushRefreshDuration = new WaitForSeconds(1.0f);

    private Vector3 dirToPlayer = new Vector3();
    private Vector3 dirToLastSeenPlayer = new Vector3();

    public float waypointMinReachDist = 5.0f;
    public float targetMinReachDist = 2.0f;

    public Transform highestObservableStructure;
    public float groundHeight = 0.0f;
    public float sturctureHeightThreshold = 3.0f;

    public float pushforce = 500.0f;

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
                    StartCoroutine(Navigate());
                }
                break;
            case AIState.PUSHING:
                if (!isPushing)
                {
                    isPushing = true;
                    StartCoroutine(Push());
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
            if(Vector3.Angle(transform.forward, dirToPlayer) <= viewingAngle)
            {
                if(playerTransform.position.y > sturctureHeightThreshold) {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, dirToPlayer, out hit, visibilityDist, aiLayerMask))
                    {
                        if (hit.collider.tag.Equals("Destructible"))
                        {
                            target = hit.transform;
                            state = AIState.NAVIGATING;
                            isWandering = false;
                        }
                    }
                }
            }          
            yield return wanderRefreshDuration;
        }
    }

    private IEnumerator Navigate()
    {
        while (isNavigating)
        {
            if(Vector3.Distance(target.position, transform.position) <= targetMinReachDist)
            {
                state = AIState.PUSHING;
                isNavigating = false;
            }
            yield return navigateRefreshDuration;
        }
    }

    private IEnumerator Push()
    {
        while (isPushing)
        {
            target.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * pushforce, ForceMode.Impulse);
            yield return pushRefreshDuration;
            if(Vector3.Distance(playerTransform.position, transform.position) < waypointMinReachDist)
            {
                transform.LookAt(playerTransform);
                if (playerTransform.position.y > sturctureHeightThreshold)
                {
                    RaycastHit[] hits = Physics.RaycastAll(transform.position, dirToPlayer, visibilityDist, aiLayerMask);
                    Vector3 lastStructurePos = new Vector3();
                    foreach(RaycastHit rh in hits)
                    {
                        if(rh.distance < waypointMinReachDist)
                        {
                            lastStructurePos = rh.transform.position;
                            rh.transform.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 10.0f, ForceMode.Impulse);
                            yield return pushRefreshDuration;
                        }
                    }
                }
                state = AIState.WANDERING;
                isPushing = false;
            }
            else
            {
                state = AIState.WANDERING;
                isPushing = false;
            }
        }
    }
}
