using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Move", menuName = "Move")]
public class Move : ScriptableObject
{
    public enum MoveType { Normal, Special }
    public MoveType type;
    public Status.BlockState collissionState;
    public enum AttackHeight { Low, Mid, High, Overhead }
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