﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnums : MonoBehaviour
{
}
public enum GroundState { Grounded, Airborne, Knockdown }
public enum BlockState { None, Standing, Crouching, Airborne }
public enum HitState { None, Knockdown, Launch };
public enum AttackHeight { Low, Mid, High, Overhead }
public enum AttackType { Normal, Projectile, Throw}
public enum MoveType { Normal, Special, Movement, EX, Super}

public enum SpecialInput { BackForward, DownDown }

public enum ButtonInput { A, B, J, C, D }