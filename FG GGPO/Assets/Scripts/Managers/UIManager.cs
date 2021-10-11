using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Gradient hpColorOverTime;
    float p1InitialHealth;
    float p2InitialHealth;

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

    public GameObject[] allUI;
    public GameObject[] versusModeObjects;
    public GameObject[] trainingModeObjects;

    public GameObject rematchScreen;
    public EventSystem eventSystem;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
        GameHandler.Instance.gameEndEvent+= DisableUI;
        GameHandler.Instance.rematchScreenEvent += RematchScreen;

        p1InitialHealth = GameHandler.Instance.p1Status.maxHealth;
        p2InitialHealth = GameHandler.Instance.p2Status.maxHealth;

    }

    public void GameModeUI(GameMode mode)
    {
        foreach (var item in trainingModeObjects)
        {
            item.SetActive(false);
        }
        foreach (var item in versusModeObjects)
        {
            item.SetActive(false);
        }
        switch (mode)
        {
            case GameMode.VersusMode:
                foreach (var item in versusModeObjects)
                {
                    item.SetActive(true);
                }
                break;
            case GameMode.TrainingMode:
                foreach (var item in trainingModeObjects)
                {
                    item.SetActive(true);
                }
                break;
            case GameMode.TutorialMode:
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void ExecuteFrame()
    {
        if (GameHandler.Instance.gameStates.Count - 1 > 0)
        {
            GameState state = GameHandler.Instance.gameStates[GameHandler.Instance.gameStates.Count - 1];
            if (GameHandler.Instance.gameMode == GameMode.VersusMode)
                timerText.text = "" + GameHandler.Instance.roundTime;
            else timerText.text = "∞";


            p1Health.value = state.p1Health;
            p2Health.value = state.p2Health;
            p1HealthText.text = state.p1Health + "/";
            p2HealthText.text = state.p2Health + "/";

            p1Meter.value = state.p1Meter;
            p2Meter.value = state.p2Meter;
            p1MeterText.text = state.p1Meter + "";
            p2MeterText.text = state.p2Meter + "";

            p1Health.targetGraphic.color = hpColorOverTime.Evaluate(1 - (float)state.p1Health / (float)p1InitialHealth);
            p2Health.targetGraphic.color = hpColorOverTime.Evaluate(1 - (float)state.p2Health / (float)p2InitialHealth);

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


    public void DisableUI() {
        foreach (var item in allUI)
        {
            item.SetActive(false);
        }
    }
    public void RematchScreen() {
        rematchScreen.SetActive(true);
    }

    public void SetActive(GameObject GO)
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(GO);
    }
}
