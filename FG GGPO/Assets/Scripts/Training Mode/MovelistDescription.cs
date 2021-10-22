using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MovelistDescription : MonoBehaviour
{
    public Move move;
    public Image moveImage;
    public TextMeshProUGUI moveName;
    public TextMeshProUGUI startupText;
    public TextMeshProUGUI activeText;
    public TextMeshProUGUI recoveryText;
    public TextMeshProUGUI hitText;
    public TextMeshProUGUI blockText;
    public TextMeshProUGUI hitCancelText;
    public TextMeshProUGUI blockCancelText;

    public Image inputImage;
    public GameObject plus;
    public Image inputButton;

    [Button]
    public void DisplayMove(Move m)
    {
        move = m;
        moveImage.sprite = move.sprite;

        moveName.text = move.moveName;
        startupText.text = "" + move.firstStartupFrame;
        string tempActive = "";
        foreach (var item in move.attacks)
        {
            tempActive += item.activeFrames + " ";
        }
        activeText.text = tempActive;
        recoveryText.text = "" + move.recoveryFrames;
        hitText.text = "" + move.hitAdvantage;
        blockText.text = "" + move.blockAdvantage;
        hitCancelText.text = "" + move.hitCancelAdvantage;
        blockCancelText.text = "" + move.blockCancelAdvantage;
    }
}
