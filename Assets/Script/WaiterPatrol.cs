using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class WaiterPatrol : MonoBehaviour
{
    [Header("Path Patrol")]
    public Transform[] pathPoints;
    public List<int> servingPoints;

    [Header("Serving Settings")]
    public float serveDuration = 3f;

    private int currentPointIndex = 0;
    private bool goingForward = true;

    private NavMeshAgent agent;
    private bool isServing = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (pathPoints.Length > 0)
            agent.SetDestination(pathPoints[currentPointIndex].position);
    }

    void Update()
    {
        animator.SetBool("isWalking", agent.velocity.magnitude > 0.1f);
        if (isServing || pathPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            if (servingPoints.Contains(currentPointIndex))
            {
                StartCoroutine(ServeCustomer());
            }
            else
            {
                MoveToNextWaypoint();
            }
        }
    }

    void MoveToNextWaypoint()
    {
        if (goingForward)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Length)
            {
                currentPointIndex = pathPoints.Length - 2;
                goingForward = false;
            }
        }
        else
        {
            currentPointIndex--;
            if (currentPointIndex < 0)
            {
                currentPointIndex = 1;
                goingForward = true;
            }
        }

        agent.SetDestination(pathPoints[currentPointIndex].position);
    }

    IEnumerator ServeCustomer()
    {
        isServing = true;
        agent.isStopped = true;
        animator.SetBool("isWalking", false); 
        animator.SetBool("isServing", true);  

        Debug.Log("NPC melayani di waypoint " + currentPointIndex);

        yield return new WaitForSeconds(serveDuration);

        Debug.Log("Selesai melayani.");

        animator.SetBool("isServing", false); 
        agent.isStopped = false;
        isServing = false;

        MoveToNextWaypoint();
    }
}
