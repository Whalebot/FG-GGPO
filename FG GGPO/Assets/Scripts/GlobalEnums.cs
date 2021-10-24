using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnums : MonoBehaviour
{
}
public enum Characters { Dynamic, Knight, Loli}
public enum GameMode { VersusMode, TrainingMode, TrialMode, TutorialMode }
public enum GroundState { Grounded, Airborne, Knockdown }
public enum BlockState { Standing, Crouching, Airborne }
public enum HitState { None, Knockdown, Launch };
public enum AttackLevel { Level1, Level2, Level3, Level4, Level5 }
public enum AttackHeight { Low, Mid, High, Overhead }
public enum BodyProperty { Foot, Body, Head, Air }
public enum AttackType { Normal, Projectile, Throw }
public enum MoveType { Normal, Special, UniversalMechanics, Movement, EX, Super }

public enum InputDirection { Neutral, Crouch, Jumping, Forward, Back, Side, JumpCrouch }
public enum SpecialInput { BackForward, DownDown, QCF, QCB, Input478, Input698 }

public enum ButtonInput { A, B, J, C, D }
public enum StagePosition { RoundStart, Wall1, Wall2, MidScreen }
