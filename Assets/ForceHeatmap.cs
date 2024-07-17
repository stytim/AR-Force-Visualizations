using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

public class ForceHeatmap : MonoBehaviour
{
    public Material mat;
    public GameObject probe;
    public GameObject body;

    public ForceSensor forceSensor;
    public float minSize = 0.06f;
    public float maxSize = 0.15f;

    public float threadholdForce = 0.6f;
    public bool showTrail = false;

    private float low = 0.2f;
    private float max = 0.4f;

    private Vector3 lastPosition = Vector3.zero;
    private Color col = Color.white;
    private float mappedSize = 0.075f;

    [SerializeField]
	private Brush brush;
    [SerializeField]
    private InkCanvas paintObject;

    private void Start()
    {

        low = forceSensor.minForce / forceSensor.stopForce;
        max = forceSensor.maxForce / forceSensor.stopForce;


        mat = body.GetComponent<Renderer>().material;

    }

    // Update is called once per frame
    void Update()
    {

        RaycastHit[] hits = Physics.RaycastAll(probe.transform.position + probe.transform.up * 0.1f, -probe.transform.up, Mathf.Infinity);

        // Iterate through all the hits
        foreach (RaycastHit hit in hits)
        {
            // Check the name of the hit object and perform specific actions
            if (hit.collider.gameObject == body)
            {            
                // Convert the hit point to local space
                Vector3 hitLocal = body.transform.InverseTransformDirection(hit.point);
                // Update the material
                mat.SetVector("_HitPosition", hitLocal);
                lastPosition = hit.point;
            }
        
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Y"))
        {
            ClearHistory();
        }


        float stickY = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.DownArrow) || stickY < 0)
        {
            //paintObject.Erase(brush,lastPosition);
            col.a = 0.1f;
            brush.Color = col;
            paintObject.Paint(brush, lastPosition); 
        }

        if (Input.GetKey(KeyCode.UpArrow) || stickY > 0)
        {
            //paintObject.Erase(brush,lastPosition);
            col.a = 0.1f;
            brush.Color = col;
            paintObject.Paint(brush, lastPosition); 
        }
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
        
        mappedSize = 0.075f - max * 0.03f / (max - low)  + normalizedForce * 0.03f/(max-low);
        mat.SetColor("_SpotColor", col);
        mat.SetFloat("_RingSize", mappedSize);

        if (showTrail)
        {
            col.a = 0.1f;
            brush.Color = col;
            paintObject.Paint(brush, lastPosition);
        }

    
    }

    public void ClearHistory()
    {
        if (paintObject!=null)
            paintObject.ResetPaint();
        mat.SetFloat("_RingSize", 0);
    }



    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated -= HandleForceUpdated;
        }

    }

    private void OnEnable()
    {
        // Ensure forceSensor is assigned, either in the Inspector or via code
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated += HandleForceUpdated;
        }

        mat.SetFloat("_DangerRadius", 0.074f);
        mat.SetFloat("_WarningRadius", 0.044f);

    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (forceSensor != null)
        {
            forceSensor.OnForceUpdated -= HandleForceUpdated;
        }

        mat.SetColor("_SpotColor", new Color(1,1,1,0));
        mat.SetFloat("_RingSize", 0);
        mat.SetFloat("_NormailizedForceMagnitude", 0);

        mat.SetFloat("_DangerRadius", 0);
        mat.SetFloat("_WarningRadius", 0);

    }

}
