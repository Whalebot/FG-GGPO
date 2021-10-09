using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New CharacterSelectProfile", menuName = "ScriptableObjects/Character Select Profile")]

public class CharacterSelectProfile : ScriptableObject
{
    [TabGroup("Ingame")]
    public GameObject prefab;
    [TabGroup("Ingame")]
    public int iD;

    [PreviewField(75)] 
    public Sprite portrait;

    [PreviewField(75)]
    public Sprite splashArt;

    public string characterName;

    public int difficulty;

    public string playstyle;

    public string description;

    public int range1;

    public int range2;

    public int range3;


}
