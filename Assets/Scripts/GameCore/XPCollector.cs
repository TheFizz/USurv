using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCollector : MonoBehaviour
{
    private Transform _myTransform;
    private PlayerSystems _pSystems;
    [SerializeField] private LayerMask _targetLayer;

    // Start is called before the first frame update
    void Awake()
    {
        _myTransform = GetComponent<Transform>();
        _pSystems = Globals.PlayerSystems;
    }

    // Update is called once per frame
    void Update()
    {
        float range = _pSystems.PlayerStats.GetStat(StatParam.PickupRange).Value;
        Collider[] hitDrops = Physics.OverlapSphere(_myTransform.position, range, _targetLayer);
        foreach (var hit in hitDrops)
        {
            string id = hit.name.Split('<')[1].Replace(">", "");
            Globals.XPDropsPool[id].Attract(_myTransform);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!Globals.IsInLayerMask(other.gameObject.layer, _targetLayer))
            return;

        other.enabled = false;
        string id = other.name.Split('<')[1].Replace(">", "");

        _pSystems.AddXP(Globals.XPDropsPool[id].XpValue);
        Globals.XPDropsPool[id].Animator.Destroy();
        Globals.XPDropsPool.Remove(id);
        Destroy(other.gameObject);
    }
}
