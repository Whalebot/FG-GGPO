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