using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killme : MonoBehaviour
{
    public float ttl = 0.2f;
    private float time = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= ttl)
            Destroy(gameObject);
    }
}
