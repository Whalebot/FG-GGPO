
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo", menuName = "ScriptableObjects/Combo")]
public class Combo : ScriptableObject
{
    public Action[] actions;
}

[System.Serializable]
public class Action {
    public Move move;
}