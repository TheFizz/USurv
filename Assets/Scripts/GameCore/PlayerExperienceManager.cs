using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperienceManager : MonoBehaviour
{
    private Transform _myTransform;
    [SerializeField] private LayerMask _targetLayer;

    // Start is called before the first frame update
    void Awake()
    {
        _myTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float range = Game.PSystems.PlayerData.GetStat(StatParam.PickupRange).Value;
        Collider[] hitDrops = Physics.OverlapSphere(_myTransform.position, range, _targetLayer);
        foreach (var hit in hitDrops)
        {
            string id = hit.name.Split('<')[1].Replace(">", "");
            Game.XPDropsPool[id].Attract(_myTransform);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!Game.IsInLayerMask(other.gameObject.layer, _targetLayer))
            return;

        other.enabled = false;
        string id = other.name.Split('<')[1].Replace(">", "");

        Game.PSystems.AddXP(Game.XPDropsPool[id].XpValue);
        Game.XPDropsPool[id].Animator.Destroy();
        Game.XPDropsPool.Remove(id);
        Destroy(other.gameObject);
    }
}
