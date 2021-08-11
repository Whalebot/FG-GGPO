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
    }
}
