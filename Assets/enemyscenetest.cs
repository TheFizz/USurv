using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyscenetest : MonoBehaviour
{
    public NewEnemyBase neb;
    public Transform t1, t2;
    // Start is called before the first frame update
    void Start()
    {
        neb.MainTarget = t1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(neb.transform.position, t1.transform.position)<=0.01)
            neb.MainTarget = t2;
        if (Vector3.Distance(neb.transform.position, t2.transform.position) <= 0.01)
            neb.MainTarget = t1;
    }
}
