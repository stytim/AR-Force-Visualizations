using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class LinearGauage : MonoBehaviour
{
    public Material mat;
    public GameObject probe;
    public GameObject body;
    public ForceSensor forceSensor;
    private float low = 0.2f;
    private float max = 0.4f;
    private Color col = Color.white;

    private List<GameObject> history;

    // Start is called before the first frame update
    void Start()
    {
        low = forceSensor.minForce / forceSensor.stopForce;
        max = forceSensor.maxForce / forceSensor.stopForce;
        history = new List<GameObject>();
    }

    private void HandleForceUpdated(float forceMagnitudeY)
    {
        float normalizedForce = Mathf.InverseLerp(0, forceSensor.stopForce, forceMagnitudeY);
        if (normalizedForce <= low)
        {
            col = Color.Lerp(forceSensor.insufficientColor, forceSensor.safeColor, Mathf.Pow(normalizedForce / low, 3f) );
        }
        else if (normalizedForce >= max)
        {
            col = Color.Lerp(forceSensor.safeColor, forceSensor.dangerColor, Mathf.Pow((normalizedForce - max) / (1 - max), 0.2f));
        }
        else
        {
            col = forceSensor.safeColor;
        }

        mat.SetColor("_FillColor", col);
        mat.SetFloat("_FillRate", normalizedForce);

    }


    public void AddTrailPoint()
    {
        if (!this.enabled)
            return;

        GameObject probe_tmp = new GameObject("Probe History");
        probe_tmp.transform.position = probe.transform.position;
        probe_tmp.transform.rotation = probe.transform.rotation;
        probe_tmp.AddComponent<MeshFilter>().sharedMesh = probe.GetComponent<MeshFilter>().sharedMesh;
        probe_tmp.AddComponent<MeshRenderer>().sharedMaterial = probe.GetComponent<MeshRenderer>().sharedMaterial;
        col.a = 0.5f;
        Material probeMat = new Material(mat)
        {
            color = col
        };
        probe_tmp.GetComponent<Renderer>().material = probeMat;
        history.Add(probe_tmp);
    }

    public void ClearHistory()
    {
        if (history == null)
            return;
        foreach (GameObject probe_tmp in history)
        {
            Destroy(probe_tmp);
        }
        history.Clear();
    }


    private void OnEnable()
    {
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated += HandleForceUpdated;
        }


        mat.SetFloat("_DangerHeight", 0.06f);
        mat.SetFloat("_WarningHeight", 0.03f);
    }

    private void OnDisable()
    {
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated -= HandleForceUpdated;
        }

        mat.SetColor("_FillColor", Color.white);
        mat.SetFloat("_FillRate", 0);
        mat.SetFloat("_DangerHeight", 0);
        mat.SetFloat("_WarningHeight", 0);

    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated -= HandleForceUpdated;
        }
    }
}
