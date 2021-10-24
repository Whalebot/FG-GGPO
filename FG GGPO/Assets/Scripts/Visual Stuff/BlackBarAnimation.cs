using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBarAnimation : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.Instance.advanceGameState += ExecuteFrame;
    }

  void ExecuteFrame()
    {
        if (!GameHandler.Instance.runNormally) anim.enabled = true;
        anim.SetBool("IsOn", GameHandler.Instance.superFlash);

        if (!GameHandler.Instance.runNormally) StartCoroutine(PauseAnimation());
    }


    IEnumerator PauseAnimation()
    {
        yield return new WaitForFixedUpdate();
        anim.enabled = false;
    }
}
