using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{

    public MoveType type;
    public BlockState collissionState;
    [FoldoutGroup("SFX")] public SFX[] sfx;
    public List<Move> gatlingMoves;
    public GameObject hitFX;
    public GameObject blockFX;
    public GameObject hitSFX;
    public GameObject blockSFX;

    public int animationID;
    public AnimationClip animationClip;
    [Header("Read Only")]
    public int firstStartupFrame;
    public int lastActiveFrame;
    public int totalMoveDuration;

    public int recoveryFrames;

    public bool canGatling;
    public bool hitSpecialCancelable;
    public bool blockSpecialCancelable;

    public Attack[] attacks;
    [Header("Move properties")]
    [FoldoutGroup("Move properties")] public int particleID;
    [FoldoutGroup("Move properties")] public bool landCancel;
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


    [Header("Momentum")]
    [FoldoutGroup("Momentum")] public bool overrideVelocity = true;
    [FoldoutGroup("Momentum")] public bool runMomentum = true;
    [FoldoutGroup("Momentum")] public bool resetVelocityDuringRecovery = true;
    [FoldoutGroup("Momentum")] public Momentum[] m;

    private void OnValidate()
    {
        if (attacks.Length <= 0) return;
        firstStartupFrame = attacks[0].startupFrame;
        lastActiveFrame = attacks[attacks.Length - 1].startupFrame + attacks[attacks.Length - 1].activeFrames - 1;
        totalMoveDuration = lastActiveFrame + recoveryFrames;
    }

    //[Button]
    //void CopyGroundToAirHit() {
    //    CopyProperty(airHitProperty, groundHitProperty);
    //    CopyProperty(airBlockProperty, groundBlockProperty);
    //}

    //[Button]
    //void CopyBaseAttack()
    //{
    ////    attacks = new Attack[1];
    //    attacks[0].hitbox = attackPrefab;
    //    attacks[0].startupFrame = startupFrames;
    //    attacks[0].activeFrames = activeFrames;
    //    attacks[0].gatlingFrames= gatlingFrames;
    //    attacks[0].attackHeight = attackHeight;

    //    CopyProperty(attacks[0].groundHitProperty, groundHitProperty);
    //    CopyProperty(attacks[0].groundCounterhitProperty, groundBlockProperty);
    //    CopyProperty(attacks[0].groundCounterhitProperty, groundCounterhitProperty);
    //    CopyProperty(attacks[0].airHitProperty, airHitProperty);
    //    CopyProperty(attacks[0].airBlockProperty, airBlockProperty);
    //    CopyProperty(attacks[0].airCounterhitProperty, airCounterhitProperty);
    //}

    //[Button]
    //void CopyCounterhit()
    //{
    //    CopyProperty(groundCounterhitProperty, groundHitProperty);
    //    CopyProperty(airCounterhitProperty, airHitProperty);
    //}

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
    public bool impulse = true;
    public int duration;
    public Vector3 momentum;
    public bool resetVelocityDuringRecovery = true;
}

[System.Serializable]
public class HitProperty
{
    public int damage;
    public int stun;
    public int hitstop;
    public float proration = 1;
    public int meterGain = 2;
    public Vector3 pushback;
    public HitState hitState;
}