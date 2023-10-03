using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXConeControl : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Material[] cone_materials;
    private ParticleSystem sparks;
   
    public float Radius;
    public float Angle;
    private float coneRotationAngle;

    void OnEnable()
    {
        foreach (Material m in cone_materials)
        {
            m.SetFloat("_Cone", Angle);
        }
        this.transform.localScale = new Vector3(Radius, 1, Radius);
        sparks = this.GetComponent<ParticleSystem>();
        coneRotationAngle= -1 * (90 - (Angle / 2));
    }
    // Update is called once per frame
    void Update()
    {
        var sparksAngle = sparks.shape;

        sparksAngle.arc = Angle%360;
        sparksAngle.rotation = new Vector3(90f, coneRotationAngle, 0f);
    }
}
