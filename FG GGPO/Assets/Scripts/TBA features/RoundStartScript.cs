using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStartScript : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        GameHandler.Instance.roundStartEvent += RoundStart;
    }

    void RoundStart() {
        anim.SetTrigger("RoundStart1");
    }
}
