using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Slider p1Health;
    public Slider p2Health;
    public TextMeshProUGUI p1HealthText;
    public TextMeshProUGUI p2HealthText;

    public Slider p1Meter;
    public Slider p2Meter;
    public TextMeshProUGUI p1MeterText;
    public TextMeshProUGUI p2MeterText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameState state = GameHandler.Instance.gameStates[GameHandler.Instance.gameStates.Count - 1];
        p1Health.value = state.p1Health;
        p2Health.value = state.p2Health;
        p1HealthText.text = state.p1Health + "/";
        p2HealthText.text = state.p2Health + "/";

        p1Meter.value = state.p1Meter;
        p2Meter.value = state.p2Meter;
        p1MeterText.text = state.p1Meter + "";
        p2MeterText.text = state.p2Meter + "";
    }
}
