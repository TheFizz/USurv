using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCollector : MonoBehaviour
{
    private Transform _myTransform;
    private PlayerSystems _pSystems;
    [SerializeField] private float _collectionRange = 20f;
    [SerializeField] private LayerMask _targetLayer;

    // Start is called before the first frame update
    void Start()
    {
        _myTransform = GetComponent<Transform>();
        _pSystems = Globals.PlayerSystems;
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
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _targetLayer) == 0)
            return;

        other.enabled = false;
        string id = other.name.Split('<')[1].Replace(">", "");

        _pSystems.AddXP(Globals.XPDropsPool[id].XpValue);
        Globals.XPDropsPool[id].Destroy();
    }
}
