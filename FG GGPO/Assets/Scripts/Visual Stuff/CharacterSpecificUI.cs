using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSpecificUI : MonoBehaviour
{
    public bool p1 = true;
    public Status status;
    public Characters character;
    public GameObject enzoUI;
    public Slider enzoSlider;
    public Gradient firelevelColor;
    EngineScript engineScript;
    // Start is called before the first frame update
    void Start()
    {
        //Enzo UI
        if (GameHandler.p1CharacterID == 1 && p1)
        {
            status = GameHandler.Instance.p1Status;
            SetupEnzoUI();
        }
        else if (GameHandler.p2CharacterID == 1 && !p1)
        {
            status = GameHandler.Instance.p2Status;
            SetupEnzoUI();
        }

        GameHandler.Instance.advanceGameState += ExecuteFrame;
    }
    public void SetupEnzoUI()
    {
        character = Characters.Knight;
        enzoUI.SetActive(true);
        engineScript = status.GetComponent<EngineScript>();
        enzoSlider.maxValue = engineScript.maxFireLevel;

    }
    // Update is called once per frame
    void ExecuteFrame()
    {
        if (character == Characters.Knight)
        {
            enzoSlider.value = engineScript.fireLevel;

            enzoSlider.targetGraphic.color = firelevelColor.Evaluate(1 - (float)engineScript.fireLevel / (float)engineScript.maxFireLevel);
        }
    }
}
