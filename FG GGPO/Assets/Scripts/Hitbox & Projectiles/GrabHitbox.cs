using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHitbox : Hitbox
{
    public Transform grabTransform;

    public override void DoDamage(Status other, float dmgMod)
    {
        base.DoDamage(other, dmgMod);
    }

    public override void CheckAttack(Status other, Attack attack)
    {
        if (other.groundState == GroundState.Grounded && other.currentState != Status.State.Hitstun && other.currentState != Status.State.Blockstun)
        {
            ExecuteThrow(attack.groundHitProperty, other);
        }
        //Check for airborne or knockdown state
        else if (other.groundState == GroundState.Airborne || other.groundState == GroundState.Knockdown)
        {
          
        }
    }

    void ExecuteThrow(HitProperty hit, Status other)
    {
        other.transform.position = grabTransform.position;
        other.transform.rotation = grabTransform.rotation;
        attack.specialCancel = move.specialCancelOnHit;
        attack.jumpCancel = move.jumpCancelOnHit;
        status.Meter += hit.meterGain;
        other.Meter += hit.meterGain / 2;

        status.GoToState(Status.State.LockedAnimation);
        other.TakeThrow(move.hitID);
        attack.Idle();
        attack.AttackProperties(move.throwFollowup);
    }
}
