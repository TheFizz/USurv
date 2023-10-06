using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform CenterPoint;
    void Update()
    {
        Vector3 pos = CenterPoint.position;
        transform.position = pos;
    }
}
