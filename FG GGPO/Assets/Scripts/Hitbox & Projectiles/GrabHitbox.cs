using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHitbox : Hitbox
{
    public bool airThrow;
    public bool groundThrow;
    public Transform grabTransform;

    public new void OnTriggerEnter(Collider other)
    {
        if (hitOnce) return;
        Status enemyStatus = other.GetComponentInParent<Status>();
        Hitbox hitbox = other.GetComponent<Hitbox>();
        colPos = other.gameObject.transform;
        if (!attack.attacking) return;
        if (enemyStatus != null)
        {
            if (status == enemyStatus) return;

            if (!enemyList.Contains(enemyStatus))
            {
                canClash = false;
                if (!CheckInvul(enemyStatus)) return;

                enemyList.Add(enemyStatus);
                DoDamage(enemyStatus, 1);
                return;
            }
        }
    }

    public override void DoDamage(Status other, float dmgMod)
    {
        base.DoDamage(other, dmgMod);
    }

    public override void CheckAttack(Status other, Attack attack)
    {
        if (airThrow && other.groundState == GroundState.Airborne)
        {
        //    if (other.currentState != Status.State.Hitstun && other.currentState != Status.State.Blockstun)
            {
                ExecuteThrow(attack.groundHitProperty, other);
            }
        }
        else if (other.groundState == GroundState.Grounded)
        {
            if (other.currentState != Status.State.Hitstun && other.currentState != Status.State.Blockstun)
            {
                ExecuteThrow(attack.groundHitProperty, other);
            }
        }

        //Check for airborne or knockdown state
        else if (other.groundState == GroundState.Airborne || other.groundState == GroundState.Knockdown)
        {

        }
    }

    void ExecuteThrow(HitProperty hit, Status other)
    {
        hitOnce = true;

        attack.specialCancel = move.specialCancelOnHit;
        attack.jumpCancel = move.jumpCancelOnHit;
        status.Meter += hit.meterGain;
        other.Meter += hit.meterGain / 2;


        attack.attacking = false;
        attack.newAttack = true;
        attack.Idle();
        status.GoToState(Status.State.LockedAnimation);
        status.rb.velocity = Vector3.zero;
        attack.ThrowLanded();
        other.TakeThrow(move.hitID);
        //
        //attack.AttackProperties(move.throwFollowup);

        other.transform.position = grabTransform.position;
        other.transform.rotation = grabTransform.rotation;

        Instantiate(VFXManager.Instance.throwFX, colPos.position, colPos.rotation);
        Instantiate(VFXManager.Instance.throwSFX, colPos.position, colPos.rotation);
    }
}
