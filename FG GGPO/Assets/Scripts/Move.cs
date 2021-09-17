using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{
    public int animationID;

    [FoldoutGroup("FX")] public SFX[] sfx;
    [FoldoutGroup("FX")] public GameObject hitFX;
    [FoldoutGroup("FX")] public GameObject blockFX;
    [FoldoutGroup("FX")] public GameObject hitSFX;
    [FoldoutGroup("FX")] public GameObject blockSFX;

    public MoveType type;
    public BlockState collissionState;

    public List<Move> gatlingMoves;
    [Header("Read Only")]
    public int firstStartupFrame;
    public int lastActiveFrame;
    public int totalMoveDuration;
    public int firstGatlingFrame;

    [Header("Frame Data")]
    public int blockAdvantage;
    public int hitAdvantage;

    [Header("Editable")]
    public int recoveryFrames;

    [Header("Move properties")]
    [FoldoutGroup("Move properties")] public bool canGatling;
    [FoldoutGroup("Move properties")] public bool jumpCancelOnBlock;
    [FoldoutGroup("Move properties")] public bool jumpCancelOnHit = true;
    [FoldoutGroup("Move properties")] public bool noClip;
    [ShowIf("noClip")]
    [FoldoutGroup("Move properties")] public int noClipStart = 1;
    [ShowIf("noClip")]
    [FoldoutGroup("Move properties")] public int noClipDuration;
    [FoldoutGroup("Move properties")] public bool invincible;
    [ShowIf("invincible")]
    [FoldoutGroup("Move properties")] public int invincibleStart = 1;
    [ShowIf("invincible")]
    [FoldoutGroup("Move properties")] public int invincibleDuration;
    [FoldoutGroup("Move properties")] public bool forceCounterhit;
    [FoldoutGroup("Air properties")] public bool useAirAction;
    [FoldoutGroup("Air properties")] public bool landCancel;
    [FoldoutGroup("Air properties")] public int landingRecovery;
    public Attack[] attacks;


    [Header("Momentum")]
    [FoldoutGroup("Momentum")] public bool overrideVelocity = true;
    [FoldoutGroup("Momentum")] public bool runMomentum = true;
    [FoldoutGroup("Momentum")] public bool resetVelocityDuringRecovery = true;
    [FoldoutGroup("Momentum")] public Momentum[] m;

    private void OnValidate()
    {
        if (attacks.Length <= 0) return;
        firstStartupFrame = attacks[0].startupFrame;
        firstGatlingFrame = attacks[0].startupFrame + attacks[0].gatlingFrames;
        lastActiveFrame = attacks[attacks.Length - 1].startupFrame + attacks[attacks.Length - 1].activeFrames - 1;
        totalMoveDuration = lastActiveFrame + recoveryFrames;
        blockAdvantage = attacks[attacks.Length - 1].groundBlockProperty.stun - (totalMoveDuration - attacks[attacks.Length - 1].startupFrame);
        hitAdvantage = attacks[attacks.Length - 1].groundHitProperty.stun - (totalMoveDuration - attacks[attacks.Length - 1].startupFrame);
        foreach (var item in attacks)
        {
            if (item.groundHitProperty.hitstop == 0)
                item.groundHitProperty.hitstop = 5;
            if (item.groundBlockProperty.hitstop == 0)
                item.groundBlockProperty.hitstop = 5;
            if (item.groundCounterhitProperty.hitstop == 0)
                item.groundCounterhitProperty.hitstop = 5;
            if (item.airHitProperty.hitstop == 0)
                item.airHitProperty.hitstop = 5;
            if (item.airBlockProperty.hitstop == 0)
                item.airBlockProperty.hitstop = 5;
            if (item.airCounterhitProperty.hitstop == 0)
                item.airCounterhitProperty.hitstop = 5;

            if (item.groundHitProperty.stun == 0)
                item.groundHitProperty.stun = 30;
            if (item.groundBlockProperty.stun == 0)
                item.groundBlockProperty.stun = 20;
            if (item.groundCounterhitProperty.stun == 0)
                item.groundCounterhitProperty.stun = 40;
            if (item.airHitProperty.stun == 0)
                item.airHitProperty.stun = 30;
            if (item.airBlockProperty.stun == 0)
                item.airBlockProperty.stun = 20;
            if (item.airCounterhitProperty.stun == 0)
                item.airCounterhitProperty.stun = 40;

            if (item.groundHitProperty.proration == 0)
                item.groundHitProperty.proration = 0.95F;
            if (item.groundBlockProperty.proration == 0)
                item.groundBlockProperty.proration = 0.95F;
            if (item.groundCounterhitProperty.proration == 0)
                item.groundCounterhitProperty.proration = 0.95F;
            if (item.airHitProperty.proration == 0)
                item.airHitProperty.proration = 0.95F;
            if (item.airBlockProperty.proration == 0)
                item.airBlockProperty.proration = 0.95F;
            if (item.airCounterhitProperty.proration == 0)
                item.airCounterhitProperty.proration = 0.95F;
        }

    }

    void CopyHitToCounterhit()
    {
        foreach (var item in attacks)
        {
            CopyProperty(item.groundCounterhitProperty, item.groundHitProperty);
            CopyProperty(item.airCounterhitProperty, item.airHitProperty);
        }

    }

    void CopyProperty(HitProperty hit1, HitProperty hit2)
    {
        FieldInfo[] defInfo1 = hit1.GetType().GetFields();
        FieldInfo[] defInfo2 = hit2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = hit1;
            object obj2 = hit2;

            object var1 = defInfo1[i].GetValue(obj);
            object var2 = defInfo2[i].GetValue(obj2);


            //ADDING VALUES
            if (var1 is int)
            {

                defInfo1[i].SetValue(obj, (int)var2);
            }
            else if (var1 is float)
            {
                defInfo1[i].SetValue(obj, (float)var2);
            }
            else if (var1 is Vector3)
            {
                defInfo1[i].SetValue(obj, (Vector3)var2);
            }
            else if (var1 is bool)
            {
                //SET VALUES
                if ((bool)var2)
                    defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
            }
        }
    }
}
[System.Serializable]
public class SFX
{
    public int startup = 1;
    public GameObject prefab;
}

[System.Serializable]
public class Attack
{
    public GameObject hitbox;
    public int startupFrame = 1;
    public int activeFrames = 1;
    public int gatlingFrames = 1;
    public AttackHeight attackHeight = AttackHeight.Mid;
    public HitProperty groundHitProperty;
    public HitProperty groundBlockProperty;
    public HitProperty groundCounterhitProperty;
    public HitProperty airHitProperty;
    public HitProperty airBlockProperty;
    public HitProperty airCounterhitProperty;
}

[System.Serializable]
public class Momentum
{
    public int startFrame = 1;
    public int duration;
    public Vector3 momentum;
    public bool homing = false;
    public bool resetVelocityDuringRecovery = true;
}

[System.Serializable]
public class HitProperty
{
    public int damage;
    public int stun = 20;
    public int hitstop = 5;
    public float proration = 0.95F;
    public int meterGain = 2;
    public Vector3 pushback;
    public HitState hitState;
}