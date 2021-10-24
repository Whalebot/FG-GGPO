using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    Status p1Status;
    Status p2Status;

    public Gradient hpColorOverTime;
    float p1InitialHealth;
    float p2InitialHealth;

    public TextMeshProUGUI timerText;


    public Image p1Portrait;
    public Image p2Portrait;
    public TextMeshProUGUI p1Name;
    public TextMeshProUGUI p2Name;
    public Slider p1Health;
    public Slider p2Health;
    public TextMeshProUGUI p1HealthText;
    public TextMeshProUGUI p2HealthText;



    public Slider p1Meter;
    public Slider p2Meter;
    public TextMeshProUGUI p1MeterText;
    public TextMeshProUGUI p2MeterText;

    public Image[] p1BurstImages;
    public Image[] p2BurstImages;

    public Image[] p1RoundWinImages;
    public Image[] p2RoundWinImages;

    public TextMeshProUGUI p1WinCounter;
    public TextMeshProUGUI p2WinCounter;

    public GameObject[] allUI;
    public GameObject[] versusModeObjects;
    public GameObject[] trainingModeObjects;
    public GameObject[] trialModeObjects;

    public GameObject rematchScreen;
    public EventSystem eventSystem;

    public Canvas canvas;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
        GameHandler.Instance.gameEndEvent += DisableUI;
        GameHandler.Instance.rematchScreenEvent += RematchScreen;
        GameHandler.Instance.superFlashStartEvent += DisableUI;
        GameHandler.Instance.superFlashEndEvent += EnableUI;
        p1Status = GameHandler.Instance.p1Status;
        p2Status = GameHandler.Instance.p2Status;

        p1InitialHealth = GameHandler.Instance.p1Status.maxHealth;
        p2InitialHealth = GameHandler.Instance.p2Status.maxHealth;

        p1Portrait.sprite = GameHandler.Instance.characters[GameHandler.p1CharacterID].portrait;
        p2Portrait.sprite = GameHandler.Instance.characters[GameHandler.p2CharacterID].portrait;

        p1Name.text = GameHandler.Instance.characters[GameHandler.p1CharacterID].name;
        p2Name.text = GameHandler.Instance.characters[GameHandler.p2CharacterID].name;

    }

    public void GameModeUI(GameMode mode)
    {
        //foreach (var item in allUI)
        //{
        //    item.SetActive(false);
        //}    
        foreach (var item in trainingModeObjects)
        {
            item.SetActive(false);
        }
        foreach (var item in versusModeObjects)
        {
            item.SetActive(false);
        }
        foreach (var item in trialModeObjects)
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
            case GameMode.TrialMode:
                foreach (var item in trialModeObjects)
                {
                    item.SetActive(true);
                }
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
            p1HealthText.text = state.p1Health + "/" + p1Status.maxHealth;
            p2HealthText.text = state.p2Health + "/" + p2Status.maxHealth;

            p1Meter.value = state.p1Meter;
            p2Meter.value = state.p2Meter;
            p1MeterText.text = state.p1Meter + "";
            p2MeterText.text = state.p2Meter + "";

            if (GameHandler.p1WinStreak > 0)
            {
                p1WinCounter.text = "Wins: " + GameHandler.p1WinStreak;
                p1WinCounter.gameObject.SetActive(true);
            }
            else p1WinCounter.gameObject.SetActive(false);

            if (GameHandler.p2WinStreak > 0)
            {
                p2WinCounter.text = "Wins: " + GameHandler.p2WinStreak;
                p2WinCounter.gameObject.SetActive(true);
            }
            else p2WinCounter.gameObject.SetActive(false);

            foreach (var item in p1BurstImages)
            {
                item.fillAmount = GameHandler.Instance.p1Status.BurstGauge / (float)6000;

                Color col = item.color;
                if (GameHandler.Instance.p1Status.BurstGauge == 6000)
                {
                    col.a = 1;
                }
                else col.a = 0.5F;

                item.color = col;
            }
            foreach (var item in p2BurstImages)
            {
                item.fillAmount = GameHandler.Instance.p2Status.BurstGauge / (float)6000;

                Color col = item.color;
                if (GameHandler.Instance.p2Status.BurstGauge == 6000)
                {
                    col.a = 1;
                }
                else col.a = 0.5F;

                item.color = col;
            }


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


    public void DisableUI()
    {
        canvas.enabled = false;
        //foreach (var item in allUI)
        //{
        //    item.SetActive(false);
        //}
    }

    public void EnableUI()
    {
        canvas.enabled = true;
    }

    public void RematchScreen()
    {
        rematchScreen.SetActive(true);
    }

    public void SetActive(GameObject GO)
    {
        GO.GetComponent<Selectable>().Select();
        //eventSystem.SetSelectedGameObject(null);
        //eventSystem.SetSelectedGameObject(GO);
        StartCoroutine(DelayActivation(GO));
    }

    IEnumerator DelayActivation(GameObject GO)
    {

        //yield return new WaitForFixedUpdate();
        yield return null;
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(GO);
    }
}
