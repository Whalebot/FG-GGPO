﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class InputReplay : MonoBehaviour
{
    public InputReplayMode replayMode;
    public bool replay;
    public InputLog log;
    public InputLog replayLog;
    public InputLog recording;
    public int recordingCounter = -1;
    public int replayID;
    public int replayStartFrame;
    public InputHandler p1Input;
    public InputHandler p2Input;

    public TextMeshProUGUI recordingText;
    public TextMeshProUGUI frameText;


    // Start is called before the first frame update
    void Start()
    {
        p1Input = GameHandler.Instance.p1Transform.GetComponent<InputHandler>();
        p2Input = GameHandler.Instance.p2Transform.GetComponent<InputHandler>();

        p1Input.R3input += InputManager.Instance.SwitchControls;
        p2Input.R3input += InputManager.Instance.SwitchControls;
        p1Input.R3input += StartRecording;
        p2Input.R3input += StopRecording;
        p1Input.L3input += PlayRecording;

        GameHandler.Instance.advanceGameState += ExecuteFrame;
        GameHandler.Instance.advanceGameState += UpdateLog;

        if (replay) LoadLog();
        recordingCounter = -1;
    }


    void ExecuteFrame()
    {
        if (replay) ReplayLog();
        switch (replayMode)
        {
            case InputReplayMode.StandBy:

                break;
            case InputReplayMode.Recording:
                UpdateRecording();
                break;
            case InputReplayMode.Replaying:
                ExecuteRecording();
                break;
            default:
                break;
        }

        UpdateText();
    }

    public void PlayRecording()
    {
        replayMode = InputReplayMode.Replaying;
        p2Input.isBot = true;
        replayStartFrame = GameHandler.Instance.gameFrameCount;
        recordingCounter = 0;
    }

    public void StartRecording()
    {
        replayMode = InputReplayMode.Recording;
        recording.inputs.Clear();

        recordingCounter = 0;
    }


    public void StopRecording()
    {
        replayMode = InputReplayMode.StandBy;

        recordingCounter = 0;
    }
    public void ExecuteRecording()
    {
        if (recording.inputs.Count > recordingCounter)
        {
            p2Input.ResolveButtons(recording.inputs[recordingCounter].buttons);
            for (int i = 0; i < p2Input.netDirectionals.Length; i++)
            {
                p2Input.netDirectionals[i] = (recording.inputs[recordingCounter].directionals)[i];
            }
            recordingCounter++;
        }
        else
        {
            replayMode = InputReplayMode.StandBy;
            recordingCounter = 0;
            p2Input.isBot = false;
        }

    }

    public void UpdateText()
    {
        switch (replayMode)
        {
            case InputReplayMode.StandBy:
                frameText.gameObject.SetActive(false);
                recordingText.text = "Stand By";
                break;
            case InputReplayMode.Recording:
                frameText.gameObject.SetActive(true);
                frameText.text = "" + recordingCounter;
                recordingText.text = "Recording";
                break;
            case InputReplayMode.Replaying:
                frameText.gameObject.SetActive(true);
                frameText.text = "" + recordingCounter;

                recordingText.text = "Play Recording";
                break;
            default:
                break;
        }
    }

    public void ReplayLog()
    {

        if (replayLog.inputs.Count > GameHandler.Instance.gameFrameCount)
        {
            if (replayID == 0)
            {
                p1Input.ResolveButtons(replayLog.inputs[GameHandler.Instance.gameFrameCount].buttons);
                for (int i = 0; i < p1Input.netDirectionals.Length; i++)
                {
                    p1Input.netDirectionals[i] = replayLog.inputs[GameHandler.Instance.gameFrameCount].directionals[i];
                }
            }
            else
            {
                p2Input.ResolveButtons(replayLog.inputs[GameHandler.Instance.gameFrameCount].buttons);
                for (int i = 0; i < p2Input.netDirectionals.Length; i++)
                {
                    p2Input.netDirectionals[i] = FlipDirections(replayLog.inputs[GameHandler.Instance.gameFrameCount].directionals)[i];
                }
            }
        }
    }

    public bool[] FlipDirections(bool[] inputs)
    {
        bool[] temp = new bool[4];

        if (inputs[0]) temp[2] = true;
        if (inputs[1]) temp[3] = true;
        if (inputs[2]) temp[0] = true;
        if (inputs[3]) temp[1] = true;
        return temp;
    }

    public void UpdateRecording()
    {
        Input temp = new Input();
        recordingCounter++;
        temp.frame = GameHandler.Instance.gameFrameCount;

        for (int i = 0; i < p2Input.heldButtons.Length; i++)
        {
            temp.buttons[i] = p2Input.heldButtons[i];
        }
        for (int i = 0; i < p2Input.netDirectionals.Length; i++)
        {
            temp.directionals[i] = p2Input.netDirectionals[i];
        }
        recording.inputs.Add(temp);
    }

    public void UpdateLog()
    {
        Input temp = new Input();
        temp.frame = GameHandler.Instance.gameFrameCount;


        for (int i = 0; i < p1Input.heldButtons.Length; i++)
        {
            temp.buttons[i] = p1Input.heldButtons[i];
        }
        for (int i = 0; i < p1Input.netDirectionals.Length; i++)
        {
            temp.directionals[i] = p1Input.netDirectionals[i];
        }
        log.inputs.Add(temp);
    }

    [Button]
    public void SaveLog()
    {
        string jsonData = JsonUtility.ToJson(log, true);
        File.WriteAllText(Application.persistentDataPath + "/inputLog.json", jsonData);
    }

    [Button]
    public void LoadLog()
    {
        replayLog = JsonUtility.FromJson<InputLog>(File.ReadAllText(Application.persistentDataPath + "/inputLog.json"));
    }

}

public enum InputReplayMode { StandBy, Recording, Replaying }
