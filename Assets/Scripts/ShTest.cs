using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShTest : MonoBehaviour
{
    public Material a;
    // Start is called before the first frame update
    void Start()
    {
        a.SetColor("_Gradient_Top", Color.white);
        a.SetColor("_Gradient_Bottom", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
