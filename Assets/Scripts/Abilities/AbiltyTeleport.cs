using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityTeleport", menuName = "Abilities/Teleport")]
public class AbiltyTeleport : AbilityBase
{
    public override void Use(Transform source)
    {
        Ray ray = Globals.MainCamera.ScreenPointToRay(Globals.InputHandler.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: TargetLayer, maxDistance: 300f))
        {
            var player = GameObject.FindWithTag("Player");
            var hitpoint = hitinfo.point;
            hitpoint.y = player.transform.position.y;
            player.transform.position = hitpoint;
        }
    }
}
