using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Tutorial", menuName = "Tutorial")]
public class TutorialSO : ScriptableObject
{
    [TextArea]
    public string tutorialName;
    [TextArea]
    public string[] tutorialDescription;
    public Action[] actions;
}
