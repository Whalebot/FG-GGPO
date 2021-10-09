using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{

    [TabGroup("Attacks")] public AttackLevel attackLevel;
    [TabGroup("Attacks")] public MoveType type;
    [TabGroup("Attacks")] 
    [ShowIf("@type == MoveType.EX || type == MoveType.Super")]
    public int meterCost;
    [TabGroup("Attacks")] public BlockState collissionState;
    [TabGroup("Attacks")] public GroundState groundState;
    
    [TabGroup("Attacks")] public Moveset stance;
    [TabGroup("Attacks")] public Move throwFollowup;
    [TabGroup("Attacks")] public Attack[] attacks;

    [TabGroup("FX")] public VFX[] vfx;
    [TabGroup("FX")] public SFX[] sfx;
    [TabGroup("FX")] public GameObject hitFX;
    [TabGroup("FX")] public GameObject blockFX;
    [TabGroup("FX")] public GameObject counterhitFX;
    [TabGroup("FX")] public GameObject hitSFX;
    [TabGroup("FX")] public GameObject blockSFX;
    [TabGroup("FX")] public GameObject counterhitSFX;

    [TabGroup("Animation")] public int animationID;
    [TabGroup("Animation")] public int hitID;
    [TabGroup("Animation")] public string moveName;
    [TabGroup("Animation")] 
    [TextArea ]public string description;

    [Header("Read Only")]
    public int firstStartupFrame;
    public int lastActiveFrame;
    public int totalMoveDuration;
    public int firstGatlingFrame;

    [Header("Frame Data")]
    public int blockAdvantage;
    public int hitAdvantage;
    public int blockCancelAdvantage;
    public int hitCancelAdvantage;

    [Header("Editable")]
    public int recoveryFrames;

    [FoldoutGroup("Momentum")]
    public Momentum[] m;

    [FoldoutGroup("Cancel properties")] public List<Move> targetComboMoves;
    [FoldoutGroup("Cancel properties")] public List<Move> gatlingMoves;
    [FoldoutGroup("Cancel properties")] public bool gatlingCancelOnBlock = true;
    [FoldoutGroup("Cancel properties")] public bool gatlingCancelOnHit = true;
    [FoldoutGroup("Cancel properties")] public bool jumpCancelOnBlock;
    [FoldoutGroup("Cancel properties")] public bool jumpCancelOnHit = true;
    [FoldoutGroup("Cancel properties")] public bool specialCancelOnBlock = true;
    [FoldoutGroup("Cancel properties")] public bool specialCancelOnHit = true;

    [FoldoutGroup("Invul properties")] public bool noClip;
    [ShowIf("noClip")]
    [FoldoutGroup("Invul properties")] public int noClipStart = 1;
    [ShowIf("noClip")]
    [FoldoutGroup("Invul properties")] public int noClipDuration;
    [FoldoutGroup("Invul properties")] public bool invincible;
    [ShowIf("invincible")]
    [FoldoutGroup("Invul properties")] public int invincibleStart = 1;
    [ShowIf("invincible")]
    [FoldoutGroup("Invul properties")] public int invincibleDuration;
    [FoldoutGroup("Invul properties")] public bool projectileInvul;
    [ShowIf("projectileInvul")]
    [FoldoutGroup("Invul properties")] public int projectileInvulStart = 1;
    [ShowIf("projectileInvul")]
    [FoldoutGroup("Invul properties")] public int projectileInvulDuration;
    [FoldoutGroup("Invul properties")] public bool linearInvul;
    [ShowIf("linearInvul")]
    [FoldoutGroup("Invul properties")] public int linearInvulStart = 1;
    [ShowIf("linearInvul")]
    [FoldoutGroup("Invul properties")] public int linearInvulDuration;
    [FoldoutGroup("Invul properties")] public bool airInvul;
    [ShowIf("airInvul")]
    [FoldoutGroup("Invul properties")] public int airInvulStart = 1;
    [ShowIf("airInvul")]
    [FoldoutGroup("Invul properties")] public int airInvulDuration;
    [FoldoutGroup("Invul properties")] public bool headInvul;
    [ShowIf("headInvul")]
    [FoldoutGroup("Invul properties")] public int headInvulStart = 1;
    [ShowIf("headInvul")]
    [FoldoutGroup("Invul properties")] public int headInvulDuration;
    [FoldoutGroup("Invul properties")] public bool bodyInvul;
    [ShowIf("bodyInvul")]
    [FoldoutGroup("Invul properties")] public int bodyInvulStart = 1;
    [ShowIf("bodyInvul")]
    [FoldoutGroup("Invul properties")] public int bodyInvulDuration;
    [FoldoutGroup("Invul properties")] public bool footInvul;
    [ShowIf("footInvul")]
    [FoldoutGroup("Invul properties")] public int footInvulStart = 1;
    [ShowIf("footInvul")]
    [FoldoutGroup("Invul properties")] public int footInvulDuration;

    [FoldoutGroup("Move properties")] public bool resetGatling = false;
    [FoldoutGroup("Move properties")] public bool noHitstopOnSelf;
    [FoldoutGroup("Move properties")] public bool crossupState;
    [FoldoutGroup("Move properties")] public bool forcedCounterHit;

    [FoldoutGroup("Projectile properties")] public int projectileLimit;
    [FoldoutGroup("Projectile properties")] public List<Move> sharedLimitProjectiles;

    [FoldoutGroup("Air properties")] public bool aimOnStartup;
    [FoldoutGroup("Air properties")] public bool useAirAction;
    [FoldoutGroup("Air properties")] public bool landCancel;
    [FoldoutGroup("Air properties")] public int landingRecovery;

    [FoldoutGroup("Momentum")] public bool overrideVelocity = true;
    [FoldoutGroup("Momentum")] public bool runMomentum = true;
    [FoldoutGroup("Momentum")] public bool resetVelocityDuringRecovery = true;

    private void OnValidate()
    {
        if (attacks.Length <= 0) return;
        firstStartupFrame = attacks[0].startupFrame;
        firstGatlingFrame = attacks[0].startupFrame + attacks[0].gatlingFrames;
        lastActiveFrame = attacks[attacks.Length - 1].startupFrame + attacks[attacks.Length - 1].activeFrames - 1;
        totalMoveDuration = lastActiveFrame + recoveryFrames;

        if (noHitstopOnSelf)
        {
            blockAdvantage = attacks[attacks.Length - 1].groundBlockProperty.stun - (totalMoveDuration - attacks[attacks.Length - 1].startupFrame) + attacks[attacks.Length - 1].groundBlockProperty.hitstop;
            hitAdvantage = attacks[attacks.Length - 1].groundHitProperty.stun - (totalMoveDuration - attacks[attacks.Length - 1].startupFrame) + attacks[attacks.Length - 1].groundHitProperty.hitstop;
        }
        else
        {
            blockAdvantage = attacks[attacks.Length - 1].groundBlockProperty.stun - (totalMoveDuration - attacks[attacks.Length - 1].startupFrame);
            hitAdvantage = attacks[attacks.Length - 1].groundHitProperty.stun - (totalMoveDuration - attacks[attacks.Length - 1].startupFrame);

            blockCancelAdvantage = attacks[attacks.Length - 1].groundBlockProperty.stun - (totalMoveDuration - attacks[attacks.Length - 1].startupFrame) + attacks[attacks.Length - 1].gatlingFrames;
            hitCancelAdvantage = attacks[attacks.Length - 1].groundHitProperty.stun - attacks[attacks.Length - 1].gatlingFrames;
        }
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

    [TabGroup("Attacks"), Button]
    void AutoAssignValues()
    {
        switch (attackLevel)
        {
            case AttackLevel.Level1:
                foreach (var item in attacks)
                {
                    item.groundHitProperty.hitstop = 8;
                    item.groundHitProperty.stun = 20;
                    item.groundHitProperty.proration = 0.75F;
                    item.groundHitProperty.meterGain = 4;

                    item.groundBlockProperty.hitstop = 8;
                    item.groundBlockProperty.stun = 15;
                    item.groundBlockProperty.proration = 0.75F;
                    item.groundBlockProperty.meterGain = 2;

                    item.groundCounterhitProperty.hitstop = 8;
                    item.groundCounterhitProperty.stun = 25;
                    item.groundCounterhitProperty.proration = 0.75F;
                    item.groundCounterhitProperty.meterGain = 4;

                    item.airHitProperty.hitstop = 8;
                    item.airHitProperty.stun = 20;
                    item.airHitProperty.proration = 0.75F;
                    item.airHitProperty.meterGain = 4;

                    item.airBlockProperty.hitstop = 8;
                    item.airBlockProperty.stun = 15;
                    item.airBlockProperty.proration = 0.75F;
                    item.airBlockProperty.meterGain = 2;

                    item.airCounterhitProperty.hitstop = 8;
                    item.airCounterhitProperty.stun = 25;
                    item.airCounterhitProperty.proration = 0.75F;
                    item.airCounterhitProperty.meterGain = 4;
                }
                break;
            case AttackLevel.Level2:
                foreach (var item in attacks)
                {
                    item.groundHitProperty.hitstop = 9;
                    item.groundHitProperty.stun = 22;
                    item.groundHitProperty.proration = 0.8F;
                    item.groundHitProperty.meterGain = 4;

                    item.groundBlockProperty.hitstop = 9;
                    item.groundBlockProperty.stun = 17;
                    item.groundBlockProperty.proration = 0.8F;
                    item.groundBlockProperty.meterGain = 2;

                    item.groundCounterhitProperty.hitstop = 9;
                    item.groundCounterhitProperty.stun = 27;
                    item.groundCounterhitProperty.proration = 0.8F;
                    item.groundCounterhitProperty.meterGain = 4;

                    item.airHitProperty.hitstop = 9;
                    item.airHitProperty.stun = 22;
                    item.airHitProperty.proration = 0.8F;
                    item.airHitProperty.meterGain = 4;

                    item.airBlockProperty.hitstop = 9;
                    item.airBlockProperty.stun = 17;
                    item.airBlockProperty.proration = 0.8F;
                    item.airBlockProperty.meterGain = 2;

                    item.airCounterhitProperty.hitstop = 9;
                    item.airCounterhitProperty.stun = 27;
                    item.airCounterhitProperty.proration = 0.8F;
                    item.airCounterhitProperty.meterGain = 4;
                }
                break;
            case AttackLevel.Level3:
                foreach (var item in attacks)
                {
                    item.groundHitProperty.hitstop = 10;
                    item.groundHitProperty.stun = 24;
                    item.groundHitProperty.proration = 0.85F;
                    item.groundHitProperty.meterGain = 4;

                    item.groundBlockProperty.hitstop = 10;
                    item.groundBlockProperty.stun = 19;
                    item.groundBlockProperty.proration = 0.85F;
                    item.groundBlockProperty.meterGain = 2;

                    item.groundCounterhitProperty.hitstop = 12;
                    item.groundCounterhitProperty.stun = 29;
                    item.groundCounterhitProperty.proration = 0.85F;
                    item.groundCounterhitProperty.meterGain = 4;

                    item.airHitProperty.hitstop = 10;
                    item.airHitProperty.stun = 24;
                    item.airHitProperty.proration = 0.85F;
                    item.airHitProperty.meterGain = 4;

                    item.airBlockProperty.hitstop = 10;
                    item.airBlockProperty.stun = 19;
                    item.airBlockProperty.proration = 0.85F;
                    item.airBlockProperty.meterGain = 2;

                    item.airCounterhitProperty.hitstop = 12;
                    item.airCounterhitProperty.stun = 29;
                    item.airCounterhitProperty.proration = 0.85F;
                    item.airCounterhitProperty.meterGain = 4;
                }
                break;
            case AttackLevel.Level4:
                foreach (var item in attacks)
                {
                    item.groundHitProperty.hitstop = 11;
                    item.groundHitProperty.stun = 25;
                    item.groundHitProperty.proration = 0.90F;
                    item.groundHitProperty.meterGain = 4;

                    item.groundBlockProperty.hitstop = 11;
                    item.groundBlockProperty.stun = 20;
                    item.groundBlockProperty.proration = 0.90F;
                    item.groundBlockProperty.meterGain = 2;

                    item.groundCounterhitProperty.hitstop = 13;
                    item.groundCounterhitProperty.stun = 30;
                    item.groundCounterhitProperty.proration = 0.90F;
                    item.groundCounterhitProperty.meterGain = 4;

                    item.airHitProperty.hitstop = 11;
                    item.airHitProperty.stun = 25;
                    item.airHitProperty.proration = 0.90F;
                    item.airHitProperty.meterGain = 4;

                    item.airBlockProperty.hitstop = 11;
                    item.airBlockProperty.stun = 20;
                    item.airBlockProperty.proration = 0.90F;
                    item.airBlockProperty.meterGain = 2;

                    item.airCounterhitProperty.hitstop = 13;
                    item.airCounterhitProperty.stun = 30;
                    item.airCounterhitProperty.proration = 0.90F;
                    item.airCounterhitProperty.meterGain = 4;
                }
                break;
            case AttackLevel.Level5:
                foreach (var item in attacks)
                {
                    item.groundHitProperty.hitstop = 12;
                    item.groundHitProperty.stun = 30;
                    item.groundHitProperty.proration = 0.95F;
                    item.groundHitProperty.meterGain = 4;

                    item.groundBlockProperty.hitstop = 12;
                    item.groundBlockProperty.stun = 20;
                    item.groundBlockProperty.proration = 0.95F;
                    item.groundBlockProperty.meterGain = 2;

                    item.groundCounterhitProperty.hitstop = 15;
                    item.groundCounterhitProperty.stun = 40;
                    item.groundCounterhitProperty.proration = 0.95F;
                    item.groundCounterhitProperty.meterGain = 4;

                    item.airHitProperty.hitstop = 12;
                    item.airHitProperty.stun = 30;
                    item.airHitProperty.proration = 0.95F;
                    item.airHitProperty.meterGain = 4;

                    item.airBlockProperty.hitstop = 12;
                    item.airBlockProperty.stun = 20;
                    item.airBlockProperty.proration = 0.95F;
                    item.airBlockProperty.meterGain = 2;

                    item.airCounterhitProperty.hitstop = 15;
                    item.airCounterhitProperty.stun = 40;
                    item.airCounterhitProperty.proration = 0.95F;
                    item.airCounterhitProperty.meterGain = 4;
                }
                break;
            default:
                break;
        }

    }

    [TabGroup("Attacks"), Button]
    void CopyHitToCounterhit()
    {
        foreach (var item in attacks)
        {
            CopyProperty(item.groundCounterhitProperty, item.groundHitProperty);
            CopyProperty(item.airCounterhitProperty, item.airHitProperty);
        }
    }

    [TabGroup("Attacks"), Button]
    void CopyGroundToAir()
    {
        foreach (var item in attacks)
        {
            CopyProperty(item.airHitProperty, item.groundHitProperty);
            CopyProperty(item.airBlockProperty, item.groundBlockProperty);
            CopyProperty(item.airCounterhitProperty, item.groundCounterhitProperty);
        }
    }
    [TabGroup("Attacks"), Button]
    void AutoSetCounterhit()
    {
        foreach (var item in attacks)
        {

            item.groundCounterhitProperty.damage = (int)(item.groundHitProperty.damage * 1.25F);
            item.groundCounterhitProperty.hitstop = item.groundHitProperty.hitstop + 5;
            item.groundCounterhitProperty.stun = item.groundHitProperty.stun + 10;
            item.groundCounterhitProperty.proration = item.airHitProperty.proration + 0.05F;
            item.groundCounterhitProperty.meterGain = item.groundHitProperty.meterGain * 2;


            item.airCounterhitProperty.damage = (int)(item.airHitProperty.damage * 1.25F);
            item.airCounterhitProperty.hitstop = item.airHitProperty.hitstop + 5;
            item.airCounterhitProperty.stun = item.airHitProperty.stun + 10;
            item.airCounterhitProperty.proration = item.airHitProperty.proration + 0.05F;
            item.airCounterhitProperty.meterGain = item.airHitProperty.meterGain * 2;
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
public class VFX
{
    public int startup = 1;
    public GameObject prefab;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;
}



[System.Serializable]
public class CustomHurtbox
{
    public int startup = 1;
    public int end = 1;
    public GameObject prefab;
}

[System.Serializable]
public class Attack
{
    public GameObject hitbox;
    public int startupFrame = 1;
    public int activeFrames = 1;
    public int gatlingFrames = 1;
    public AttackType attackType = AttackType.Normal;
    public AttackHeight attackHeight = AttackHeight.Mid;
    public BodyProperty bodyProperty = BodyProperty.Body;
    public bool homing;
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
    public int hitID = 0;
}