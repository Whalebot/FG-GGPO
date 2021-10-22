using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Moveset", menuName = "ScriptableObjects/Moveset")]
public class Moveset : ScriptableObject
{
    [FoldoutGroup("A Buttons")] [Header("A buttons")] public Move sA;
    [FoldoutGroup("B Buttons")] [Header("B buttons")] public Move sB;
    [FoldoutGroup("C Buttons")] [Header("C buttons")] public Move sC;

    [FoldoutGroup("A Buttons")] public Move cA;
    [FoldoutGroup("B Buttons")] public Move cB;
    [FoldoutGroup("C Buttons")] public Move cC;

    [FoldoutGroup("A Buttons")] public Move fA;
    [FoldoutGroup("B Buttons")] public Move fB;
    [FoldoutGroup("C Buttons")] public Move fC;

    [FoldoutGroup("A Buttons")] public Move bA;
    [FoldoutGroup("B Buttons")] public Move bB;
    [FoldoutGroup("C Buttons")] public Move bC;

    [FoldoutGroup("A Buttons")] public Move LA;
    [FoldoutGroup("A Buttons")] public Move RA;
    [FoldoutGroup("B Buttons")] public Move LB;
    [FoldoutGroup("B Buttons")] public Move RB;
    [FoldoutGroup("C Buttons")] public Move LC;
    [FoldoutGroup("C Buttons")] public Move RC;

    [FoldoutGroup("A Buttons")] public Move jA;
    [FoldoutGroup("A Buttons")] public Move jcA;
    [FoldoutGroup("B Buttons")] public Move jB;
    [FoldoutGroup("B Buttons")] public Move jcB;
    [FoldoutGroup("C Buttons")] public Move jC;
    [FoldoutGroup("C Buttons")] public Move jcC;

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
    public Move grabF;
    [FoldoutGroup("Movement Options")]
    [Header("Movement Options")]
    public Move backDash;
    [FoldoutGroup("Movement Options")] public Move rightDash;
    [FoldoutGroup("Movement Options")] public Move leftDash;

    [FoldoutGroup("Movement Options")] public Move airdashF;
    [FoldoutGroup("Movement Options")] public Move airdashB;

    [FoldoutGroup("Wakeup Options")] public Move neutralTech;
    [FoldoutGroup("Wakeup Options")] public Move backTech;
    [FoldoutGroup("Wakeup Options")] public Move forwadTech;
    [FoldoutGroup("Wakeup Options")] public Move leftTech;
    [FoldoutGroup("Wakeup Options")] public Move rightTech;
    [FoldoutGroup("Wakeup Options")] public Move airTech;
    [FoldoutGroup("Wakeup Options")] public Move airFTech;
    [FoldoutGroup("Wakeup Options")] public Move airBTech;

    public Move burst;



    [Header("Specials")]
    public SpecialMove[] specials;
}

[System.Serializable]
public class SpecialMove
{
    public SpecialInput motionInput;
    public ButtonInput buttonInput;
    public bool grounded;
    public Move move;
}