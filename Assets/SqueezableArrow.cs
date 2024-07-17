using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqueezeArrow : MonoBehaviour
{
    public ForceSensor forceSensor;
    private float low = 0.2f;
    private float max = 0.4f;

    public GameObject arrow;

    private List<GameObject> history;
    Color col = Color.white;

    public Material mat;

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
            col = Color.Lerp(forceSensor.insufficientColor, forceSensor.safeColor, Mathf.Pow(normalizedForce / low, 3f));
        }
        else if (normalizedForce >= max)
        {
            col = Color.Lerp(forceSensor.safeColor, forceSensor.dangerColor, Mathf.Pow((normalizedForce - max) / (1 - max), 0.2f));
        }
        else
        {
            col = forceSensor.safeColor;
        }
        mat.SetColor("_Color", col);

        // Update the scale
        Vector3 scale = arrow.transform.localScale;
        arrow.transform.localScale = new Vector3(scale.x, 2f - normalizedForce * 2f, scale.z);

    }


    public void AddTrailPoint()
    {
        if (!this.enabled)
            return;

        GameObject arrow_tmp = Instantiate(arrow, arrow.transform.position, arrow.transform.rotation);

        col.a = 0.5f;
        Material arroweMat = new Material(mat)
        {
            color = col
        };
        arrow_tmp.GetComponentInChildren<Renderer>().material = arroweMat;
        history.Add(arrow_tmp);
    }

    public void ClearHistory()
    {
        if (history == null)
            return;
        foreach (GameObject arrow_tmp in history)
        {
            Destroy(arrow_tmp);
        }
        history.Clear();
        Vector3 scale = arrow.transform.localScale;
        arrow.transform.localScale = new Vector3(scale.x, 0, scale.z);
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated -= HandleForceUpdated;
        }

    }


    private void OnDisable()
    {
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated -= HandleForceUpdated;
        }

        arrow.SetActive(false);
    }

    private void OnEnable()
    {
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated += HandleForceUpdated;
        }
        arrow.SetActive(true);
    }
}
