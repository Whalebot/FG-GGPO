using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineScript : MonoBehaviour
{
    public int fireLevel;
    public int maxFireLevel;
    public int justFrameWindow;
    public bool checkJustFrame;
    public int justFrameCounter;
    public int justFrameGain;
    public InputHandler input;
    public AttackScript attackScript;
    public GameObject engineFX;
    public GameObject engineVFX;
    public Moveset fireMoveset;
    public FireMove[] fireMoves;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
        attackScript.attackPerformedEvent += ResetCheck;
    }

    private void ResetCheck(Move move)
    {
        foreach (var item in fireMoves)
        {
            if (item.move == move)
            {
                fireLevel -= item.fireCost;
                fireLevel = Mathf.Clamp(fireLevel, 0, maxFireLevel);
                return;
            }
        }
        checkJustFrame = true;
        justFrameCounter = justFrameWindow / 2;
    }

    // Update is called once per frame
    void ExecuteFrame()
    {
        fireLevel -= 1;
        fireLevel = Mathf.Clamp(fireLevel, 0, maxFireLevel);

        if (fireLevel > 300) attackScript.moveset = fireMoveset;
        if (attackScript.attacking && checkJustFrame)
        {
            if (attackScript.attackFrames == attackScript.gatlingFrame)
            {
                if (justFrameCounter < 0)
                {
                    checkJustFrame = false;
                    return;
                }
                justFrameCounter--;
                for (int i = 0; i <= justFrameWindow / 2; i++)
                {
                    if (input.inputLog.Count < i - 2) return;
                    if (input.inputLog[input.inputLog.Count - i - 1].buttons[4] && !input.inputLog[input.inputLog.Count - i - 2].buttons[4])
                    {
                        Instantiate(engineFX, transform.position, transform.rotation);
                        Instantiate(engineVFX, transform.position + Vector3.up * 0.5f, transform.rotation);
                        fireLevel += justFrameGain;
                        fireLevel = Mathf.Clamp(fireLevel, 0, maxFireLevel);
                        checkJustFrame = false;
                        return;
                    }
                }
            }
        }
        else checkJustFrame = false;
    }
}

[System.Serializable]
public class FireMove
{
    public Move move;
    public int fireCost;
}