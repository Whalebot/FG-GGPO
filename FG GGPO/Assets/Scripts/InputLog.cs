using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputLog
{
    public List<Input> inputs;
}

[System.Serializable]
public class Input
{
    public int frame;
    public bool[] directionals = new bool[4];
    public bool[] buttons = new bool[6];
}
