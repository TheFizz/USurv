using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject _centerPoint;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = _centerPoint.transform.position;
        transform.position = pos;
    }
}
