using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] LayerMask _targetLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (Globals.IsInLayerMask(other.transform.gameObject.layer, _targetLayer))
            Game.ReloadScene();
    }
}
