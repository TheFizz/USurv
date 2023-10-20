using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXStabConeControl : MonoBehaviour, IConeControl
{
    // Start is called before the first frame update
    [SerializeField] private Material[] cone_materials;
    private ParticleSystem spears;

    public float Radius { get; set; }
    public float Angle { get; set; }
    private float coneRotationAngle;
    [SerializeField] private ParticleSystem sparks;
    [SerializeField] private GameObject cone;

    void OnEnable()
    {
        foreach (Material m in cone_materials)
        {
            m.SetFloat("_Cone", Angle);
           
        }
        cone.transform.localScale = new Vector3(Radius, 1, Radius);
        spears = this.GetComponent<ParticleSystem>();
        coneRotationAngle = -1 * (90 - (Angle / 2));
        
    }
    // Update is called once per frame
    void Update()
    {
        var spearsAngle = spears.shape;
        var spearsAmmount = spears.emission.rateOverTime;
        var spearsRadius = spears.shape.radius;
        var sparkPoint = sparks.shape.position;

        spearsAngle.arc = Angle % 360;
        spearsAngle.rotation = new Vector3(90f, coneRotationAngle, 0f);
        spearsAmmount = Angle * 0.75f;
        spearsRadius = Radius - 5f;
        sparkPoint.z = Radius;
        
    }
}
