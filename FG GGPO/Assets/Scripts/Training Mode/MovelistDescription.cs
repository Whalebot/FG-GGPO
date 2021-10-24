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
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI blockDamageText;
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
        if (move == null)
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
            return;
        }
        else
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(true);
        }

        moveImage.sprite = move.sprite;

        moveName.text = move.moveName;



        string tempStartup = "";
        string tempActive = "";
        string tempDamage = "";
        string tempChipDamage = "";
        foreach (var item in move.attacks)
        {
            tempStartup += item.startupFrame + " ";
            tempActive += item.activeFrames + " ";
            tempDamage += item.groundHitProperty.damage + " ";
            tempChipDamage += item.groundBlockProperty.damage + " ";
        }
        startupText.text = tempStartup;
        damageText.text = tempDamage;
        blockDamageText.text = tempChipDamage;
        activeText.text = tempActive;
        recoveryText.text = "" + move.recoveryFrames;
        hitText.text = "" + move.hitAdvantage;
        blockText.text = "" + move.blockAdvantage;
        hitCancelText.text = "" + move.hitCancelAdvantage;
        blockCancelText.text = "" + move.blockCancelAdvantage;
    }
}
