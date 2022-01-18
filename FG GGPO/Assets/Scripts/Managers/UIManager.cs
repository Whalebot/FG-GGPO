using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    Status p1Status;
    Status p2Status;

    public Gradient hpColorOverTime;
    float p1InitialHealth;
    float p2InitialHealth;

    [FoldoutGroup("Components")] public TextMeshProUGUI timerText;



    [FoldoutGroup("Components")] public Image p1Portrait;
    [FoldoutGroup("Components")] public Image p2Portrait;
    [FoldoutGroup("Components")] public TextMeshProUGUI p1Name;
    [FoldoutGroup("Components")] public TextMeshProUGUI p2Name;
    [FoldoutGroup("Components")] public Slider p1Health;
    [FoldoutGroup("Components")] public Slider p2Health;
    [FoldoutGroup("Components")] public TextMeshProUGUI p1HealthText;
    public TextMeshProUGUI p2HealthText;


    public Gradient meterColorOverTime;
    [FoldoutGroup("Components")] public Slider p1Meter;
    [FoldoutGroup("Components")] public Slider p2Meter;
    [FoldoutGroup("Components")] public TextMeshProUGUI p1MeterText;
    [FoldoutGroup("Components")] public TextMeshProUGUI p2MeterText;

    [FoldoutGroup("Components")] public Image[] p1BurstImages;
    [FoldoutGroup("Components")] public Image[] p2BurstImages;

    [FoldoutGroup("Components")] public Image[] p1RoundWinImages;
    [FoldoutGroup("Components")] public Image[] p2RoundWinImages;

    [FoldoutGroup("Components")] public TextMeshProUGUI p1WinCounter;
    [FoldoutGroup("Components")] public TextMeshProUGUI p2WinCounter;

    public GameObject[] allUI;
    public GameObject[] versusModeObjects;
    public GameObject[] trainingModeObjects;
    public GameObject[] trialModeObjects;    
    public GameObject[] tutorialModeObjects;

    public Canvas[] canvasses;
    public GameObject[] toggleObjects;
        
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

        GameHandler.Instance.p1IntroEvent += DisableUI;
        GameHandler.Instance.roundStartEvent+= EnableUI;
        p1Status = GameHandler.Instance.p1Status;
        p2Status = GameHandler.Instance.p2Status;

        p1InitialHealth = GameHandler.Instance.p1Status.maxHealth;
        p2InitialHealth = GameHandler.Instance.p2Status.maxHealth;

        p1Health.maxValue = p1InitialHealth;
        p2Health.maxValue = p2InitialHealth;


        p1Portrait.sprite = GameHandler.Instance.characters[GameHandler.p1CharacterID].portrait;
        p2Portrait.sprite = GameHandler.Instance.characters[GameHandler.p2CharacterID].portrait;

        p1Name.text = GameHandler.Instance.characters[GameHandler.p1CharacterID].characterName;
        p2Name.text = GameHandler.Instance.characters[GameHandler.p2CharacterID].characterName;
        ExecuteFrame();
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

        if (GameHandler.Instance.gameMode == GameMode.VersusMode)
            timerText.text = "" + GameHandler.Instance.roundTime;
        else timerText.text = "∞";


        p1Health.value = p1Status.Health;
        p2Health.value = p2Status.Health;
        p1HealthText.text = p1Status.Health + "/" + p1Status.maxHealth;
        p2HealthText.text = p2Status.Health + "/" + p2Status.maxHealth;

        p1Meter.value = p1Status.Meter/100;
        p2Meter.value = p2Status.Meter / 100;
        p1MeterText.text = p1Status.Meter / 100 + "";
        p2MeterText.text = p2Status.Meter / 100 + "";

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


        p1Health.targetGraphic.color = hpColorOverTime.Evaluate(1 - (float)p1Status.Health / (float)p1InitialHealth);
        p2Health.targetGraphic.color = hpColorOverTime.Evaluate(1 - (float)p2Status.Health / (float)p2InitialHealth);

        p1Meter.targetGraphic.color = meterColorOverTime.Evaluate(1 - (float)p1Status.meter / 100);
        p2Meter.targetGraphic.color = meterColorOverTime.Evaluate(1 - (float)p2Status.meter / 100);


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
