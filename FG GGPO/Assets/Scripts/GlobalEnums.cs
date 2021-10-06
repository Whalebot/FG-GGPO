using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnums : MonoBehaviour
{
}
public enum GameMode { VersusMode, TrainingMode, TutorialMode }
public enum GroundState { Grounded, Airborne, Knockdown }
public enum BlockState {Standing, Crouching, Airborne }
public enum HitState { None, Knockdown, Launch };
public enum AttackLevel { Level1, Level2, Level3, Level4, Level5 }
public enum AttackHeight { Low, Mid, High, Overhead }
public enum BodyProperty { Foot, Body, Head, Air }
public enum AttackType { Normal, Projectile, Throw }
public enum MoveType { Normal, Special, Movement, EX, Super }

public enum SpecialInput { BackForward, DownDown }

public enum ButtonInput { A, B, J, C, D }