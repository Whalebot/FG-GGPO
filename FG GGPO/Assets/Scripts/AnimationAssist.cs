using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAssist : MonoBehaviour
{
    Movement mov;
    AttackScript attack;

    public float animationEventWeight = 0.5F;
    // Start is called before the first frame update
    void Start()
    {
        mov= GetComponentInParent<Movement>();
       attack = GetComponentInParent<AttackScript>();
    }

    // Update is called once per frame

    void Recovery(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.Recovery();
        }
    }


    void Idle(AnimationEvent evt)
    {
      if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.Idle();
        }
    }

    void ForceIdle(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            if(attack.attacking) attack.Idle();
        }
    }

    void StartRotation(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.StartRotation();
        }
    }


    void StopRotation(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.StopRotation();
        }
    }

    void AttackLink(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.AttackLink();
        }
    }

    void Active(AnimationEvent evt)
    {
       if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.Active();
        }
    }

    void JumpFX() { }

    void StartMomentum() {
        attack.AttackMomentum();
    }

    void ParticleStart(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.ParticleStart();
        }
    }

    void ParticleStop(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.ParticleEnd();
        }
    }

    void Momentum(AnimationEvent evt)
    {
       // if (evt.animatorClipInfo.weight > animationEventWeight)
        {
            attack.AnimMomemtun();
        }
    }
}
