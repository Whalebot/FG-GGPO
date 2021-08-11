﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Moveset", menuName = "ScriptableObjects/Moveset")]
public class Moveset : ScriptableObject
{
    public Move A5;
    public Move B5;
    public Move C5;

    public Move cA;
    public Move cB;
    public Move cC;

    public Move jA;
    public Move jB;
    public Move jC;

    public Move D5;
    public Move cD;
    public Move jD;

    public Move backDash;
    public Move rightDash;
    public Move leftDash;
}

