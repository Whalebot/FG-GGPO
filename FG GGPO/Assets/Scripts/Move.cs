﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{
    public enum MoveType { Normal, Special }
    public MoveType type;
    public BlockState collissionState;

    public AttackHeight attackHeight;

    public List<Move> gatlingMoves;
    public GameObject attackPrefab;
    public GameObject hitFX;
    public GameObject blockFX;

    public int animationID;
    public AnimationClip animationClip;

    public int startupFrames;
    public int activeFrames;
    public int recoveryFrames;
    public int gatlingFrames;

    public bool canGatling;
    public bool hitSpecialCancelable;
    public bool blockSpecialCancelable;

    [Header("Hit properties")]
    public int damage;
    public float hitStun;
    public float blockStun;
    public Vector3 hitPushback;
    public Vector3 airHitPushback;
    public Vector3 blockPushback;


    public HitProperty groundHitProperty;
    public HitProperty groundCounterhitProperty;
    public HitProperty airHitProperty;
    public HitProperty airCounterhitProperty;



    [Header("Block Stop")]
    [FoldoutGroup("Feedback")] public float blockSlowMotion = 0.01F;
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

    [Header("Momentum")]
    [FoldoutGroup("Momentum")] public bool overrideVelocity = true;
    [FoldoutGroup("Momentum")] public bool runMomentum = true;
    [FoldoutGroup("Momentum")] public bool resetVelocityDuringRecovery = true;
    [FoldoutGroup("Momentum")] public Momentum[] m;

}

[System.Serializable]
public class Momentum {
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
    public Vector3 pushback;
    public HitState hitState;
}