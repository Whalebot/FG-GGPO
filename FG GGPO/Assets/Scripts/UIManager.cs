using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    public Slider p1Health;
    public Slider p2Health;
    public TextMeshProUGUI p1HealthText;
    public TextMeshProUGUI p2HealthText;

    public Slider p1Meter;
    public Slider p2Meter;
    public TextMeshProUGUI p1MeterText;
    public TextMeshProUGUI p2MeterText;

    public Image[] p1RoundWinImages;
    public Image[] p2RoundWinImages;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameHandler.Instance.gameStates.Count - 1 > 0)
        {
            GameState state = GameHandler.Instance.gameStates[GameHandler.Instance.gameStates.Count - 1];
            timerText.text = "" + GameHandler.Instance.roundTime;

            p1Health.value = state.p1Health;
            p2Health.value = state.p2Health;
            p1HealthText.text = state.p1Health + "/";
            p2HealthText.text = state.p2Health + "/";

            p1Meter.value = state.p1Meter;
            p2Meter.value = state.p2Meter;
            p1MeterText.text = state.p1Meter + "";
            p2MeterText.text = state.p2Meter + "";
        }

        for (int i = 0; i < p1RoundWinImages.Length; i++)
        {
            p1RoundWinImages[i].gameObject.SetActive(i < GameHandler.Instance.p1RoundWins);
        }
        for (int i = 0; i < p2RoundWinImages.Length; i++)
        {
            p2RoundWinImages[i].gameObject.SetActive(i < GameHandler.Instance.p2RoundWins);
        }
    }
}
