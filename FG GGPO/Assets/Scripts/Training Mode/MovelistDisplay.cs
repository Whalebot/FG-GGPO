using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MovelistDisplay : MonoBehaviour
{
    public Move move;
    public TextMeshProUGUI moveName;
    public Image directionImage;
    public GameObject jumping;
    public GameObject plus;
    public Image buttonImage;
    public Image crouchImage;

    public Sprite[] fSprites;
    public Sprite[] rSprites;
    public Sprite[] lSprites;
    public Sprite[] bSprites;

    public Sprite[] l1Sprites;
    public Sprite[] r1Sprites;
    public Sprite[] westSprites;
    public Sprite[] northSprites;
    public Sprite[] eastSprites;
    public Sprite[] southSprites;


    public GameObject attackHeightGO;
    public TextMeshProUGUI attackHeightText;

    public TextMeshProUGUI meterCost;

    [Button]
    public void SetupMove(Move m)
    {
        move = m;
        moveName.text = move.moveName;
        jumping.SetActive(false);
        directionImage.gameObject.SetActive(true);
        crouchImage.gameObject.SetActive(false);
        plus.SetActive(true);
        meterCost.gameObject.SetActive(move.meterCost > 0);
        meterCost.text = "Meter cost: " + move.meterCost;
        if (m.type != MoveType.Movement && m.type != MoveType.UniversalMechanics)
            attackHeightText.text = "" + m.attacks[0].attackHeight;
        else {
            attackHeightGO.SetActive(false);
        }
        switch (move.moveDirection)
        {
            case InputDirection.Neutral:
                directionImage.gameObject.SetActive(false);
                plus.SetActive(false);
                break;
            case InputDirection.Crouch:
                directionImage.gameObject.SetActive(false);
                crouchImage.gameObject.SetActive(true);
                break;
            case InputDirection.Jumping:
                directionImage.gameObject.SetActive(false);
                plus.SetActive(false);
                jumping.SetActive(true);
                break;
            case InputDirection.Forward:
                directionImage.sprite = fSprites[0];
                break;
            case InputDirection.Back:
                directionImage.sprite = bSprites[0];
                break;
            case InputDirection.Side:
                directionImage.sprite = rSprites[0];
                break;
            case InputDirection.JumpCrouch:
                crouchImage.gameObject.SetActive(true);
                jumping.SetActive(true);
                break;
            default:
                break;
        }
        switch (move.moveButton)
        {
            case ButtonInput.A:
                buttonImage.sprite = westSprites[0];
                break;
            case ButtonInput.B:
                buttonImage.sprite = northSprites[0];
                break;
            case ButtonInput.J:
                break;
            case ButtonInput.C:
                buttonImage.sprite = eastSprites[0];
                break;
            case ButtonInput.D:
                buttonImage.sprite = r1Sprites[0];
                break;
            default:
                break;
        }


        //inputImage.sprite =
    }

    public void Selected()
    {
        GetComponentInParent<MovelistManager>().DescriptionWindow(move);
    }

}
