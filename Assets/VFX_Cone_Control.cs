using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Cone_Control : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Material[] cone_materials;
    private ParticleSystem sparks;
   
    public float radius;
    public float angle;
    private float coneRotationAngle;

    void OnEnable()
    {
        foreach (Material m in cone_materials)
        {
            m.SetFloat("_Cone", angle);
        }
        this.transform.localScale = new Vector3(radius, 1, radius);
        sparks = this.GetComponent<ParticleSystem>();
        coneRotationAngle= -1 * (90 - (angle / 2));
        
        
    }
    // Update is called once per frame
    void Update()
    {
        var sparksAngle = sparks.shape;

        sparksAngle.arc = angle%360;
        sparksAngle.rotation = new Vector3(90f, coneRotationAngle, 0f);
    }
}
