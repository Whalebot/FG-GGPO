using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;


public class InputReplay : MonoBehaviour
{
    public bool replay;
    public InputLog log;
    public InputLog replayLog;
    public int replayID;
    public InputHandler p1Input;
    public InputHandler p2Input;

    // Start is called before the first frame update
    void Start()
    {
        p1Input = GameHandler.Instance.p1Transform.GetComponent<InputHandler>();
        p2Input = GameHandler.Instance.p2Transform.GetComponent<InputHandler>();

        GameHandler.Instance.advanceGameState += ExecuteFrame;
        GameHandler.Instance.advanceGameState += UpdateLog;

        if (replay) LoadLog();
    }


    void ExecuteFrame()
    {
        if (replay) ReplayLog();
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
