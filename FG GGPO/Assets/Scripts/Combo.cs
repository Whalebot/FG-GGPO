using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Combo", menuName = "ScriptableObjects/Combo")]
public class Combo : ScriptableObject
{
    [TextArea]
    public string description;
    public StagePosition startPosition;
    public InputLog comboDemonstration;
    public Action[] actions;
}

[System.Serializable]
public class Action {
    public enum ActionType { Performed, Hit, Jump}
    public ActionType type;
    [HideIf("@type == ActionType.Jump")]public Move move;
}