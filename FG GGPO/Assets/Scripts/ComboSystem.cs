﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }
    public float comboDamageBaseProration;
    public int healthBarRevertFrames;

    public int p1ComboCounter;
    public int p2ComboCounter;
    Status p1;
    Status p2;
    public int comboDisplayDuration;

    public delegate void ComboEvent();
    public ComboEvent p1ComboEndEvent;
    public ComboEvent p2ComboEndEvent;

    public Image p1HealthFeedback;
    public Image p2HealthFeedback;

    [TabGroup("Damage Display")] public TextMeshProUGUI p1ComboText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p2ComboText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p1ComboDamageText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p2ComboDamageText;

    [TabGroup("Training Mode")] public TextMeshProUGUI p1DamageText;
    [TabGroup("Training Mode")] public TextMeshProUGUI p2DamageText;
    [TabGroup("Training Mode")] public TextMeshProUGUI p1ComboDamageTrainingText;
    [TabGroup("Training Mode")] public TextMeshProUGUI p2ComboDamageTrainingText;
    [TabGroup("Training Mode")] public TextMeshProUGUI p1MaxComboText;
    [TabGroup("Training Mode")] public TextMeshProUGUI p2MaxComboText;

    [TabGroup("Proration Display")] public TextMeshProUGUI p1ProrationText;
    [TabGroup("Proration Display")] public TextMeshProUGUI p2ProrationText;
    [TabGroup("Proration Display")] public Slider p1ProrationSlider;
    [TabGroup("Proration Display")] public Slider p2ProrationSlider;


    [FoldoutGroup("On Screen Popups")] public GameObject p1CounterhitText;
    [FoldoutGroup("On Screen Popups")] public GameObject p2CounterhitText;

    [FoldoutGroup("On Screen Popups")] public GameObject p1PunishText;
    [FoldoutGroup("On Screen Popups")] public GameObject p2PunishText;

    [FoldoutGroup("On Screen Popups")] public GameObject p1InvincibleText;
    [FoldoutGroup("On Screen Popups")] public GameObject p2InvincibleText;

    [FoldoutGroup("On Screen Popups")] public GameObject p1ReversalText;
    [FoldoutGroup("On Screen Popups")] public GameObject p2ReversalText;

    bool p1ComboEnd;
    bool p2ComboEnd;

    int p1LastHP;
    int p2LastHP;

    public int p1Max;
    public int p2Max;

    public int p1Average;
    public int p2Average;

    int p1Last;
    int p2Last;

    public List<int> p1ComboDamages;
    public List<int> p2ComboDamages;

    int p1Counter;
    int p2Counter;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        p1 = GameHandler.Instance.p1Status;
        p2 = GameHandler.Instance.p2Status;

        p1.hitEvent += UpdateP1ComboCounter;
        p2.hitEvent += UpdateP2ComboCounter;
        p1.counterhitEvent += P1Counterhit;
        p2.counterhitEvent += P2Counterhit;
        p1.punishEvent += P1Punish;
        p2.punishEvent += P2Punish;
        p1.invincibleEvent += P1Invincible;
        p2.invincibleEvent += P2Invincible;
        p1.reversalEvent += P1Reversal;
        p2.reversalEvent += P2Reversal;

        p1.recoveryEvent += P1ComboEnd;
        p2.recoveryEvent += P2ComboEnd;

        GameHandler.Instance.advanceGameState += ExecuteFrame;
        GameHandler.Instance.roundStartEvent += ResetValues;

        p1LastHP = p1.maxHealth;
        p2LastHP = p2.maxHealth;

        p1ComboDamages = new List<int>();
        p2ComboDamages = new List<int>();
    }
    public void ResetValues()
    {
        p1Max = 0;
        p2Max = 0;

        p1Average = 0;
        p2Average = 0;

        p1ComboDamages.Clear();
        p2ComboDamages.Clear();
    }

    public void ExecuteFrame()
    {
        p1ComboCounter = p1.comboCounter;
        p2ComboCounter = p2.comboCounter;


        p1Counter--;
        p2Counter--;

        p1ProrationSlider.value = (float)p1.HitStun / 60F;
        p2ProrationSlider.value = (float)p2.HitStun / 60F;
        p1LastHP = p1.Health;
        p2LastHP = p2.Health;
        if (p1Counter <= 0)
        {
            p1PunishText.SetActive(false);
            p1InvincibleText.SetActive(false);
            p1ComboText.gameObject.SetActive(false);
            p1CounterhitText.SetActive(false);

            p1ReversalText.SetActive(false);
            ResetP1Combo();
            p1HealthFeedback.fillAmount =
                Mathf.Lerp(p1HealthFeedback.fillAmount, GameHandler.Instance.p1Status.Health / (float)GameHandler.Instance.p1Status.maxHealth, (float)Mathf.Clamp(Mathf.Abs(p1Counter), 0, healthBarRevertFrames) / healthBarRevertFrames);
        }

        if (p2Counter <= 0)
        {
            p2PunishText.SetActive(false);
            p2ComboText.gameObject.SetActive(false);
            p2CounterhitText.SetActive(false);
            p2InvincibleText.SetActive(false);
            p2ReversalText.SetActive(false);
            ResetP2Combo();
            p2HealthFeedback.fillAmount = Mathf.Lerp(p2HealthFeedback.fillAmount, GameHandler.Instance.p2Status.Health / (float)GameHandler.Instance.p2Status.maxHealth, (float)Mathf.Clamp(Mathf.Abs(p2Counter), 0, healthBarRevertFrames) / healthBarRevertFrames);
        }
    }

    public void ResetP1Combo()
    {
        // p1HealthFeedback.fillAmount = p1LastHP / (float)GameHandler.Instance.p1Status.maxHealth;


        p1ComboEndEvent?.Invoke();

    }

    public void ResetP2Combo()
    {
        // p2HealthFeedback.fillAmount = p2LastHP / (float)GameHandler.Instance.p2Status.maxHealth;

        p2ComboEndEvent?.Invoke();

    }

    void P1ComboEnd()
    {
        p1ComboEnd = true;

    }
    void P2ComboEnd()
    {
        p2ComboEnd = true;

    }

    public void P1Invincible()
    {
        p1Counter = comboDisplayDuration;

        p1InvincibleText.SetActive(true);
    }

    public void P2Invincible()
    {
        p2Counter = comboDisplayDuration;

        p2InvincibleText.SetActive(true);
    }
    public void P1Reversal()
    {
        p1ReversalText.SetActive(true);
    }

    public void P2Reversal()
    {

        p2ReversalText.SetActive(true);
    }
    public void P1Punish()
    {

        p1PunishText.SetActive(true);
    }
    public void P2Punish()
    {

        p2PunishText.SetActive(true);
    }
    public void P1Counterhit()
    {

        p2CounterhitText.SetActive(true);
    }

    public void P2Counterhit()
    {
        p1CounterhitText.SetActive(true);
    }
    public void UpdateP1ComboCounter()
    {
        if (p1ComboEnd)
        {
            p1ComboDamages.Add(p1Last);
            int sum = 0;
            foreach (var item in p1ComboDamages)
            {
                sum += item;
            }
            p2Average = sum / p1ComboDamages.Count;
            ResetP1Combo();
            p1HealthFeedback.fillAmount = p1LastHP / (float)GameHandler.Instance.p1Status.maxHealth;
            p1ComboEnd = false;
        }

        p1Counter = comboDisplayDuration;
        p1ComboText.gameObject.SetActive(true);

        p1ComboText.text = p1.comboCounter + " HITS";
        p1ComboDamageText.text = "" + p1.comboDamage;

        p1Last = p1.comboDamage;
        p2ComboDamageTrainingText.text = "" + p1.comboDamage;
        p2DamageText.text = "" + p1.lastAttackDamage;

        if (p1.comboDamage > p2Max)
            p2Max = p1.comboDamage;

        p2MaxComboText.text = "" + p2Max;
        p1ProrationText.text = "" + p1.proration;

    }
    public void UpdateP2ComboCounter()
    {
        if (p2ComboEnd)
        {
            p2ComboDamages.Add(p2Last);
            int sum = 0;
            foreach (var item in p2ComboDamages)
            {
                sum += item;
            }
            p1Average = sum / p2ComboDamages.Count;
            ResetP2Combo();
            p2HealthFeedback.fillAmount = p2LastHP / (float)GameHandler.Instance.p2Status.maxHealth;
            p2ComboEnd = false;
        }

        p2Counter = comboDisplayDuration;
        p2ComboText.gameObject.SetActive(true);
        print("s");

        p2ComboText.text = p2.comboCounter + " HITS";
        p2ComboDamageText.text = "" + p2.comboDamage;

        p2Last = p2.comboDamage;
        p1ComboDamageTrainingText.text = "" + p2.comboDamage;
        p1DamageText.text = "" + p2.lastAttackDamage;

        if (p2.comboDamage > p1Max)
            p1Max = p2.comboDamage;

        p1MaxComboText.text = "" + p1Max;
        p2ProrationText.text = "" + p2.proration;
    }
}
