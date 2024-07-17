using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class ForceSensor : MonoBehaviour
{
    public float maxForce = 10.0f;
    public float minForce = 5f;
    public float stopForce = 25f;
    public float factor = 20f;
    public Color insufficientColor = Color.blue;
    public Color safeColor = Color.green;
    public Color dangerColor = Color.red;

    private bool sensorStarted = false;

    public delegate void ForceUpdateHandler(float force);
    public event ForceUpdateHandler OnForceUpdated;

    private float forceMagnitudeY = 0;
    // Structure to hold force and timestamp
    public struct ForceData
    {
        public float Force;
        public DateTime Timestamp;

        public ForceData(float force, DateTime timestamp)
        {
            Force = force;
            Timestamp = timestamp;
        }
    }

    // List to store force data
    private List<ForceData> forceDataList = new List<ForceData>();

    private void Start()
    {
        
    }

    private void Update()
    {
        if (sensorStarted)
        {
            OnForceUpdated?.Invoke(forceMagnitudeY);
        }
        
    }

    public void StartSimulatedSensor()
    {
        StartCoroutine(SimulateForceChange());
        sensorStarted = true;
    }

    private IEnumerator SimulateForceChange()
    {
        float duration = 10.0f;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            if (elapsedTime < duration / 3)
            {
                // Interpolating from 0 to 15
                forceMagnitudeY = Mathf.Lerp(0, 15, elapsedTime / (duration / 3));
            }
            else if (elapsedTime < 2 * duration / 3)
            {
                // Interpolating from 15 to 5
                forceMagnitudeY = Mathf.Lerp(15, 3, (elapsedTime - duration / 3) / (duration / 3));
            }
            else
            {
                // Interpolating from 5 to 20
                forceMagnitudeY = Mathf.Lerp(3, 15, (elapsedTime - 2 * duration / 3) / (duration / 3));
            }

            forceDataList.Add(new ForceData(forceMagnitudeY, DateTime.Now));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        forceMagnitudeY = 0;
        sensorStarted = false;

    }

}

