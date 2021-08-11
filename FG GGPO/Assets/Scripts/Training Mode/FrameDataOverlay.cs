﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FrameDataOverlay : MonoBehaviour
{
    public Status status;
    public AttackScript attack;

    public TextMeshProUGUI startupText;
    public TextMeshProUGUI frameAdvantageText;
    public TextMeshProUGUI blockStateText;
    public TextMeshProUGUI hitStateText;
    public TextMeshProUGUI groundStateText;

    // Start is called before the first frame update
    void Start()
    {
        //attack.startupEvent += UpdateStartup;
        
    }

    private void FixedUpdate()
    {
        UpdateStates();
        
    }

    void UpdateStartup() {
        if(attack.activeMove != null)
        startupText.text = attack.gameFrames + " (" + attack.activeMove.startupFrames +  " -" + (attack.activeMove.startupFrames - 1 + attack.activeMove.activeFrames + ")");
    }
    public void UpdateAdvantage(int frames)
    {
        print(attack.iFrames + " call 2");
        UpdateStartup();
        UpdateStates();
        if (frames > 0) frameAdvantageText.text = "+" + frames;
        else
        frameAdvantageText.text = "" +  frames;
    }

    // Update is called once per frame
    void UpdateStates()
    {
        blockStateText.text = "" + status.blockState;
        hitStateText.text = "" + status.currentState;
        groundStateText.text = "" + status.groundState;
    }
}