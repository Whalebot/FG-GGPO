using System.Collections;
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

    public void UpdateStartup() {
        if(attack.activeMove != null)
        startupText.text = attack.attackFrames + " (" + attack.activeMove.firstStartupFrame +  "-" + attack.activeMove.lastActiveFrame + ")";
        else { }
    }

    public void UpdateAdvantage(int frames, int cancelFrames)
    {
        UpdateStates();
        if (frames > 0) frameAdvantageText.text = "+" + frames + " (" + cancelFrames + ")";
        else
        frameAdvantageText.text = "" + frames + " (" + cancelFrames + ")";
    }

    // Update is called once per frame
    void UpdateStates()
    {
        blockStateText.text = "" + status.blockState.ToString();
        hitStateText.text = "" + status.currentState;
        groundStateText.text = "" + status.groundState;
    }
}
