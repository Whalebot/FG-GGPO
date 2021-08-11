using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{
    public enum MoveType { Normal, Special }
    public MoveType type;
    public enum AttackHeight { Low, Mid, High, Overhead }
    public AttackHeight attackHeight;

    public Move[] gatlingMoves;
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

    public enum HitProperty { None, Knockdown, Launch }
    public HitProperty groundHitProperty;
    public HitProperty airHitProperty;

    [Header("Screen shake")]
    [FoldoutGroup("Feedback")] public float shakeMagnitude;
    [FoldoutGroup("Feedback")] public float shakeDuration;
    [Header("Hit Stop")]
    [FoldoutGroup("Feedback")] public float slowMotionDuration = 0.01F;
    [FoldoutGroup("Feedback")] public bool startupParticle;
    [Header("Block Stop")]
    [FoldoutGroup("Feedback")] public float blockSlowMotion = 0.01F;
    [Header("Move properties")]
    [FoldoutGroup("Move properties")] public bool verticalRotation = true;
    [FoldoutGroup("Move properties")] public int particleID;
    [FoldoutGroup("Move properties")] public bool landCancel;
    [FoldoutGroup("Move properties")] public bool holdAttack;
    [FoldoutGroup("Move properties")] public bool tracking;
    [FoldoutGroup("Move properties")] public bool homing;
    [FoldoutGroup("Move properties")] public bool fullCancelable;
    [FoldoutGroup("Move properties")] public bool noClip;
    [FoldoutGroup("Move properties")] public bool iFrames;

    [Header("Momentum")]
    [FoldoutGroup("Momentum")] public bool overrideVelocity = true;
    [FoldoutGroup("Momentum")] public bool resetVelocityDuringRecovery = true;
    [FoldoutGroup("Momentum")] public Vector3 Momentum;
    [FoldoutGroup("Momentum")] public Vector3[] momentumArray;

}
