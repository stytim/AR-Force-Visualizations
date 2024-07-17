using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanningPathControl : MonoBehaviour
{
    public Transform[] waypoints;  // Array of waypoints for the object to follow
    private int currentWaypointIndex = 0;  // Index of the current waypoint
    private float[] moveDurations = { 2f, 10f, 2f };  // Durations to move between waypoints
    private float moveStartTime;  // The time at which the movement started
    private bool isMoving = true;  // Flag to control movement

    // Start is called before the first frame update
    void Start()
    {
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;  // Start at the first waypoint
            moveStartTime = Time.time;  // Initialize the start time
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving && waypoints.Length > 1)
        {
            MoveTowardsWaypoint();
        }
    }

    void MoveTowardsWaypoint()
    {
        if (currentWaypointIndex < moveDurations.Length)
        {
            // Calculate the interpolation factor
            float t = (Time.time - moveStartTime) / moveDurations[currentWaypointIndex];
            // Move towards the next waypoint using linear interpolation
            transform.position = Vector3.Lerp(waypoints[currentWaypointIndex].position, waypoints[currentWaypointIndex + 1].position, t);

            // Check if the movement duration has elapsed
            if (t >= 1f)
            {
                // Move to the next waypoint
                currentWaypointIndex++;
                if (currentWaypointIndex < moveDurations.Length)
                {
                    moveStartTime = Time.time;  // Reset the start time for the next segment

                    if (currentWaypointIndex == 1)
                    {
                       GetComponent<ForceSensor>().StartSimulatedSensor();
                    }
                }
                else
                {
                    isMoving = false;  // Stop moving after reaching the last waypoint
                }
            }
        }
    }

    // Public function to restart movement from the first point
    public void RestartMovement()
    {
        if (waypoints.Length > 0)
        {
            currentWaypointIndex = 0;  // Reset to the first waypoint
            transform.position = waypoints[0].position;  // Move to the first waypoint
            moveStartTime = Time.time;  // Reset the start time
            isMoving = true;  // Resume movement
        }
    }
}
