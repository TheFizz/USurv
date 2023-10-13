using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityDash", menuName = "Abilities/Dash")]
public class AbiltyDash : AbilityBase
{
    public override void Use(Transform source)
    {
        Vector3 hitpoint = Vector3.up;
        Ray ray = Game.MainCamera.ScreenPointToRay(Game.InputHandler.MousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, layerMask: TargetLayer, maxDistance: 300f))
        {
            var player = GameObject.FindWithTag("Player");
            hitpoint = hitinfo.point;
            hitpoint.y = player.transform.position.y;
        }

        var power = GetStat(StatParam.AbilityPower).Value;
        var distance = GetStat(StatParam.AbilityRange).Value;
        var time = distance / power;

        if (hitpoint == Vector3.up)
            return;
        var force = (hitpoint - Game.PSystems.PlayerObject.transform.position).normalized * power;
        Game.PSystems.MovementController.ForceMoveGhost(force, time);
    }
}
