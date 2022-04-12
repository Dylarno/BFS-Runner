using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlindEnemyBehaviour : Entity
{
    public Transform[] waypoints;
    private int currentWaypoint;

    private void Update()
    {
        DoMovement();
    }

    private void DoMovement()
    {
        // don't move if dead
        if (!isAlive)
            return;

        // don't move if already moving
        if (isMoving)
            return;

        // get the next waypoint as the target position
        currentWaypoint = (currentWaypoint + 1 >= waypoints.Length) ? 0 : currentWaypoint + 1;
        Vector3 targetPos = waypoints[currentWaypoint].position;

        // move to the next waypointt
        StartCoroutine(Move(targetPos));
    }
}
