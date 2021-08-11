﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameDataManager : MonoBehaviour
{
    public Status p1;
    public Status p2;
    public FrameDataOverlay overlay1;
    public FrameDataOverlay overlay2;
    public int frame;
    // Start is called before the first frame update
    void Start()
    {
       // if (p1 == null)
        {
            p1 = GameHandler.Instance.p1Status;
            p2 = GameHandler.Instance.p2Status;

            overlay1.status = p1;
            overlay2.status = p2;

            overlay1.attack = p1.GetComponent<AttackScript>();
            overlay2.attack = p2.GetComponent<AttackScript>();
        }

        overlay1.attack.startupEvent += UpdateHit;
        overlay2.attack.startupEvent += UpdateHit;
        p1.hitEvent += UpdateHit;
        p2.hitEvent += UpdateHit;
        p1.frameDataEvent += UpdateFrameData;
        p2.frameDataEvent += UpdateFrameData;
    }

    void UpdateHit()
    {
        frame = p1.minusFrames - p2.minusFrames;
        overlay1.UpdateStartup();
        overlay2.UpdateStartup();
        overlay1.UpdateAdvantage(frame);
        overlay2.UpdateAdvantage(-frame);
    }

    // Update is called once per frame
    void UpdateFrameData()
    {
        frame = p1.minusFrames - p2.minusFrames;
        overlay1.UpdateAdvantage(frame);
        overlay2.UpdateAdvantage(-frame);
    }
}
