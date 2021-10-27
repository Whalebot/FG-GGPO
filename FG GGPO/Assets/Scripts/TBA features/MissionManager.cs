using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }
    public InputReplay replayer;
    public int comboStep;
    public bool clear;
    public Combo activeComboTrial;
    public TextMeshProUGUI comboName;
    public int comboTrialID;
    public Combo[] comboTrials;
    public Status playerStatus;
    public Status dummyStatus;
    public AttackScript playerAttack;
    public AttackScript dummyAttack;
    public GameObject comboUIPrefab;
    public Transform prefabContainer;
    public List<ActionUI> comboTrialObjects;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerStatus = GameHandler.Instance.p1Status;
        dummyStatus = GameHandler.Instance.p2Status;
        playerAttack = playerStatus.GetComponent<AttackScript>();
        dummyAttack = dummyStatus.GetComponent<AttackScript>();

        playerAttack.jumpEvent += JumpPerformed;
        playerAttack.attackHitEvent += ActionHit;
        playerAttack.attackPerformedEvent += ActionPerformed;
        dummyStatus.recoveryEvent += FailedMission;
        GameHandler.Instance.resetEvent += FailedMission;
        comboTrials = GameHandler.Instance.characters[GameHandler.p1CharacterID].comboTrials;
        ChangeComboTrial(0);
    }

    [Button]
    public void NextComboTrial()
    {
        if (comboTrials.Length > comboTrialID + 1)
            ChangeComboTrial(comboTrialID + 1);
    }

    public void ChangeComboTrial(int id)
    {
        comboTrialID = id;
        activeComboTrial = comboTrials[id];
        comboName.text = activeComboTrial.description;

        GameHandler.Instance.startPosition = activeComboTrial.startPosition;
        InputLog log = new InputLog();
        log.inputs = new List<Input>();

        playerStatus.meter = activeComboTrial.meter;
        dummyStatus.forcedCounterhit = activeComboTrial.counterhit;
        foreach (var item in activeComboTrial.comboDemonstration.inputs)
        {
            log.inputs.Add(item);
        }
        replayer.SetRecording(log);
        SetupUI();
        GameHandler.Instance.ResetRound();
    }

    public void ActionHit(Move move)
    {
        if (activeComboTrial.actions[comboStep].type == Action.ActionType.Hit)
            if (activeComboTrial.actions[comboStep].move == move)
            {
                UpdateUI();
                comboStep++;
                if (comboStep >= activeComboTrial.actions.Length)
                    ComboClear();
            }
    }
    public void ActionPerformed(Move move)
    {
        if (activeComboTrial.actions[comboStep].type == Action.ActionType.Performed)
            if (activeComboTrial.actions[comboStep].move == move)
            {
                UpdateUI();
                comboStep++;
                if (comboStep >= activeComboTrial.actions.Length)
                    ComboClear();
            }
    }
    public void JumpPerformed()
    {
        if (activeComboTrial.actions[comboStep].type == Action.ActionType.Jump)
        {
            UpdateUI();
            comboStep++;
            if (comboStep >= activeComboTrial.actions.Length)
                ComboClear();
        }
    }

    [Button]
    void SetupUI()
    {
        foreach (var item in comboTrialObjects)
        {
            Destroy(item.gameObject);
        }
        comboTrialObjects.Clear();

        foreach (var item in activeComboTrial.actions)
        {
            GameObject GO = Instantiate(comboUIPrefab, prefabContainer);
            ActionUI actionUI = GO.GetComponent<ActionUI>();
            actionUI.Setup(item.move);
            comboTrialObjects.Add(actionUI);
        }
    }
    void UpdateUI()
    {
        comboTrialObjects[comboStep].Performed();

    }
    void ComboClear()
    {
        comboStep = 0;
        clear = true;

    }

    public void FailedMission()
    {
        comboStep = 0;
        foreach (var item in comboTrialObjects)
        {
            item.ResetUI();
        }
    }
}

