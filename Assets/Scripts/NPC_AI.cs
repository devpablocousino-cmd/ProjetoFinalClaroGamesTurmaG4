using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class NPC_AI : MonoBehaviour
{

    [Header("Components")]
    public List<Transform> wayPoints;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;
    public LayerMask playerLayer;
    public GameObject dialogue;

    [Header("Variables")]
    public int currentWayPointIndex = 0;
    public float speed = 2f;
    private bool isPlayerDetected = false;
    private bool onRadious;

    void Start()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = speed;
    }

    void Update()
    {
        if (!isPlayerDetected)
        {
            Walking();
        }
        else
        {
            stopMalking();
        }
    }
    private void Walking()
    {
        if (wayPoints.Count == 0)
        {
            return;
        }
        float distanceTowaypoint = Vector3.Distance(
            wayPoints[currentWayPointIndex].position,
            transform.position);

        if (distanceTowaypoint <= 2)
        {
            currentWayPointIndex = (currentWayPointIndex + 1) % wayPoints.Count;
        }

        navMeshAgent.SetDestination(wayPoints[currentWayPointIndex].position);
        onRadious = false;
    }

    private void stopMalking()
    {
        navMeshAgent.isStopped = true;
        onRadious = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = true;
            dialogue.SetActive(true);
            Debug.Log("Player Detected");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = false;
            navMeshAgent.isStopped = false;
            dialogue.SetActive(false);
        }
    }
}