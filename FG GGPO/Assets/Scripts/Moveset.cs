using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Moveset", menuName = "ScriptableObjects/Moveset")]
public class Moveset : ScriptableObject
{
    [Header("A buttons")]
    public Move sA;
    public Move cA;
    public Move fA;
    public Move bA;
    public Move jA;
    public Move jcA;
    [Header("B buttons")]

    public Move sB;
    public Move cB;
    public Move fB;
    public Move bB;
    public Move jB;
    public Move jcB;

    [Header("C buttons")]
    public Move sC;
    public Move cC;
    public Move fC;
    public Move bC;
    public Move jC;
    public Move jcC;

    [Header("D buttons")]
    public Move sD;
    public Move cD;
    public Move bD;
    public Move fD;
    public Move jD;
    public Move jcD;

    [Header("Movement Options")]
    public Move backDash;
    public Move rightDash;
    public Move leftDash;

    public Move airdashF;
    public Move airdashB;

    [Header("Specials")]
    public SpecialMove[] specials;
}

[System.Serializable]
public class SpecialMove {
    public SpecialInput motionInput;
    public ButtonInput buttonInput;
    public Move move;
}