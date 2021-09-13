using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }
    public float proration;
    public int p1ComboCounter;
    public int p2ComboCounter;
    Status p1;
    Status p2;
    public int comboDisplayDuration;
    public TextMeshProUGUI p1ComboText;
    public TextMeshProUGUI p2ComboText;

    public TextMeshProUGUI p1DamageText;
    public TextMeshProUGUI p2DamageText;
    public TextMeshProUGUI p1ComboDamageText;
    public TextMeshProUGUI p2ComboDamageText;
    public TextMeshProUGUI p1MaxComboText;
    public TextMeshProUGUI p2MaxComboText;

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
    }

    // Update is called once per frame
    void Update()
    {
        p1ComboCounter = p1.comboCounter;
        p2ComboCounter = p2.comboCounter;
    }

    private void FixedUpdate()
    {
        p1Counter--;
        p2Counter--;
        if (p1Counter <= 0) { p1ComboText.gameObject.SetActive(false); }
        if (p2Counter <= 0) { p2ComboText.gameObject.SetActive(false); }
    }

    public void UpdateP1ComboCounter()
    {
        p1Counter = comboDisplayDuration;
        p1ComboText.gameObject.SetActive(true);
        p1ComboText.text = p1.comboCounter + " HITS";

        p2ComboDamageText.text = "" + p1.comboDamage;
        p2DamageText.text =  "" + p1.lastAttackDamage;

        if (p1.comboDamage > p1Max)
            p1Max = p1.comboDamage;
        p2MaxComboText.text = "" + p1Max;
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

    }
}
