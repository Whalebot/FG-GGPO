using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackOverlayAnimation : MonoBehaviour
{
    public Animator anim;
    bool resetting;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
        GameHandler.Instance.resetEvent += ResetEvent;
        GameHandler.Instance.roundStartEvent += ResetEvent;
    }

    void ResetEvent()
    {
        anim.SetTrigger("IsOn");
    }

    void ExecuteFrame()
    {
        //if (!GameHandler.Instance.runNormally) anim.enabled = true;
        //if (!GameHandler.Instance.runNormally) StartCoroutine(PauseAnimation());
    }


    IEnumerator PauseAnimation()
    {
        yield return new WaitForFixedUpdate();
        anim.enabled = false;
    }
}
