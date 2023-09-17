using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCollector : MonoBehaviour
{
    private Transform _myTransform;
    [SerializeField] private float _collectionRange = 20f;
    [SerializeField] private LayerMask _targetLayer;
    // Start is called before the first frame update
    void Start()
    {
        _myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitDrops = Physics.OverlapSphere(_myTransform.position, _collectionRange, _targetLayer);
        foreach (var hit in hitDrops)
        {
            var drop = hit.GetComponent<XPDrop>();
            drop.Attract(_myTransform);
        }
    }
}
