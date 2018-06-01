using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinotaurController : MonoBehaviour {

    public Camera cam;
    public NavMeshAgent agent;
    public Transform[] points;
    public GameObject player;
    public float radious;
    private int destPoint = 0;

    public Transform playerSpawnPoint;
    public int eaten = 0;

    [SerializeField] private bool huntingPlayer = false;
	

	// Update is called once per frame
	void Update () {

        // get the distance from player
        float distanceFromPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceFromPlayer < radious)
        {
            huntingPlayer = true;
            agent.SetDestination(player.transform.position);
        }
        else if (distanceFromPlayer > radious)
        {
            huntingPlayer = false;
        }
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f && huntingPlayer == false)
            GotoNextPoint();
    }


    void Start()
    {
        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radious);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Destroy(other.gameObject);
        other.transform.position = playerSpawnPoint.position;
        print("Eaten");
        eaten++;
        huntingPlayer = false;
    }
}
