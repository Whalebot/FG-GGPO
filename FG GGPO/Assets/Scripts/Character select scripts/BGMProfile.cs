using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New BGMProfile", menuName = "ScriptableObjects/BGM Profile")]

public class BGMProfile : ScriptableObject
{
    public int ID;
    public string bgmName;
    public AudioClip bgmClip;
    public Sprite characterPortrait;
}
