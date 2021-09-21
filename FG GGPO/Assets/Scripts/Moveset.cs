using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Moveset", menuName = "ScriptableObjects/Moveset")]
public class Moveset : ScriptableObject
{
    [FoldoutGroup("A Buttons")]
    [Header("A buttons")]
    [FoldoutGroup("A Buttons")] public Move sA;
    [FoldoutGroup("A Buttons")] public Move cA;
    [FoldoutGroup("A Buttons")] public Move fA;
    [FoldoutGroup("A Buttons")] public Move bA;
    [FoldoutGroup("A Buttons")] public Move jA;
    [FoldoutGroup("A Buttons")] public Move jcA;
    [FoldoutGroup("A Buttons")] public Move LA;
    [FoldoutGroup("A Buttons")] public Move RA;
    [FoldoutGroup("B Buttons")]
    [Header("B buttons")]
    [FoldoutGroup("B Buttons")] public Move sB;
    [FoldoutGroup("B Buttons")] public Move cB;
    [FoldoutGroup("B Buttons")] public Move fB;
    [FoldoutGroup("B Buttons")] public Move bB;
    [FoldoutGroup("B Buttons")] public Move jB;
    [FoldoutGroup("B Buttons")] public Move jcB;
    [FoldoutGroup("B Buttons")] public Move LB;
    [FoldoutGroup("B Buttons")] public Move RB;

    [FoldoutGroup("C Buttons")]
    [Header("C buttons")]
    public Move sC;
    [FoldoutGroup("C Buttons")] public Move cC;
    [FoldoutGroup("C Buttons")] public Move fC;
    [FoldoutGroup("C Buttons")] public Move bC;
    [FoldoutGroup("C Buttons")] public Move jC;
    [FoldoutGroup("C Buttons")] public Move jcC;
    [FoldoutGroup("C Buttons")] public Move LC;
    [FoldoutGroup("C Buttons")] public Move RC;
    [FoldoutGroup("D Buttons")]
    [Header("D buttons")]
    public Move sD;
    [FoldoutGroup("D Buttons")] public Move cD;
    [FoldoutGroup("D Buttons")] public Move bD;
    [FoldoutGroup("D Buttons")] public Move fD;
    [FoldoutGroup("D Buttons")] public Move jD;
    [FoldoutGroup("D Buttons")] public Move jcD;
    [FoldoutGroup("D Buttons")] public Move LD;
    [FoldoutGroup("D Buttons")] public Move RD;

    [FoldoutGroup("Movement Options")]
    [Header("Movement Options")]
    public Move backDash;
    [FoldoutGroup("Movement Options")] public Move rightDash;
    [FoldoutGroup("Movement Options")] public Move leftDash;

    [FoldoutGroup("Movement Options")] public Move airdashF;
    [FoldoutGroup("Movement Options")] public Move airdashB;

    [Header("Specials")]
    public SpecialMove[] specials;
}

[System.Serializable]
public class SpecialMove
{
    public SpecialInput motionInput;
    public ButtonInput buttonInput;
    public Move move;
}