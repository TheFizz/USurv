using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject _centerPoint;
    // Update is called once per frame
    private void Awake()
    {
        Globals.MainCamera = GetComponentInChildren<Camera>();
    }
    private void Start()
    {
        _centerPoint = Globals.PlayerTransform.Find("AttackSource").gameObject;
    }
    void Update()
    {
        Vector3 pos = _centerPoint.transform.position;
        transform.position = pos;
    }
}
