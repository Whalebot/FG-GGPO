using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }
    public float comboDamageBaseProration;
    public int p1ComboCounter;
    public int p2ComboCounter;
    Status p1;
    Status p2;
    public int comboDisplayDuration;

    [TabGroup("Damage Display")] public TextMeshProUGUI p1ComboText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p2ComboText;

    [TabGroup("Damage Display")] public TextMeshProUGUI p1DamageText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p2DamageText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p1ComboDamageText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p2ComboDamageText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p1MaxComboText;
    [TabGroup("Damage Display")] public TextMeshProUGUI p2MaxComboText;

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

    int p1Max;
    int p2Max;

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

        GameHandler.Instance.advanceGameState += ExecuteFrame;
    }

    // Update is called once per frame
    void Update()
    {
        p1ComboCounter = p1.comboCounter;
        p2ComboCounter = p2.comboCounter;
    }

    private void FixedUpdate()
    {

    }

    public void ExecuteFrame() {
        p1Counter--;
        p2Counter--;
        if (p1Counter <= 0) { 
            p1ComboText.gameObject.SetActive(false); 
            p1CounterhitText.SetActive(false);
            p1InvincibleText.SetActive(false);
            p1ReversalText.SetActive(false);
            p1PunishText.SetActive(false);
        }
        if (p2Counter <= 0) { 
            p2ComboText.gameObject.SetActive(false); 
            p2CounterhitText.SetActive(false);
            p2InvincibleText.SetActive(false);
            p2ReversalText.SetActive(false);
            p2PunishText.SetActive(false);

        }

        p1ProrationSlider.value = (float)p1.HitStun / 60F;
        p2ProrationSlider.value = (float)p2.HitStun / 60F;
    }
    public void P1Invincible()
    {
        p1InvincibleText.SetActive(true);
    }

    public void P2Invincible()
    {
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
        p1Counter = comboDisplayDuration;
        p1ComboText.gameObject.SetActive(true);
        p1ComboText.text = p1.comboCounter + " HITS";

        p2ComboDamageText.text = "" + p1.comboDamage;
        p2DamageText.text = "" + p1.lastAttackDamage;

        if (p1.comboDamage > p1Max)
            p1Max = p1.comboDamage;
        p2MaxComboText.text = "" + p1Max;
        p1ProrationText.text = "" + p1.proration;
    }
    public void UpdateP2ComboCounter()
    {
        p2Counter = comboDisplayDuration;
        p2ComboText.gameObject.SetActive(true);
        p2ComboText.text = p2.comboCounter + " HITS";

        p1ComboDamageText.text = "" + p2.comboDamage;
        p1DamageText.text = "" + p2.lastAttackDamage;

        if (p2.comboDamage > p2Max)
            p2Max = p2.comboDamage;
        p1MaxComboText.text = "" + p2Max;
        p2ProrationText.text = "" + p2.proration;
    }
}
