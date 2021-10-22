using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New StageProfile", menuName = "ScriptableObjects/Stage Profile")]

public class StageProfile : ScriptableObject
{
    public int ID;
    public string stageName;
    public Sprite thumbnail;
    public Sprite preview;
    public Sprite overview;
}
