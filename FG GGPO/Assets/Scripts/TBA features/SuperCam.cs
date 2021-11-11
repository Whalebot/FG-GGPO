using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperCam : MonoBehaviour
{
    AttackScript attack;
    CharacterAnimator animatorScript;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
 
        GameHandler.Instance.advanceGameState += ExecuteFrame;
        anim = GetComponent<Animator>();
        attack = GetComponentInParent<AttackScript>();
        GameHandler.Instance.resetEvent += DestroyMe;
        attack.superFlashEndEvent += DestroyMe;
    }

    // Update is called once per frame
    void ExecuteFrame()
    {
        if (!GameHandler.Instance.runNormally) anim.enabled = true;
        //if (status.hitstopCounter > 0)

        //else
        //    anim.speed = 1;
        if (!GameHandler.Instance.runNormally) StartCoroutine(PauseAnimation());
    }
    IEnumerator PauseAnimation()
    {
        yield return new WaitForFixedUpdate();
        anim.enabled = false;
    }
    void DestroyMe()
    {
        attack.superFlashEndEvent -= DestroyMe;
        GameHandler.Instance.resetEvent -= DestroyMe;
        GameHandler.Instance.advanceGameState -= ExecuteFrame;
        Destroy(gameObject);

    }
}
