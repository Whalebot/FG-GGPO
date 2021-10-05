using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameDataManager : MonoBehaviour
{
    public static FrameDataManager Instance { get; private set; }
    public Status p1;
    public Status p2;
    public FrameDataOverlay overlay1;
    public FrameDataOverlay overlay2;
    public int frame;
    public int cancelFrame;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

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

        overlay1.attack.startupEvent += P1UpdateHit;
        overlay2.attack.startupEvent += P2UpdateHit;
        p1.hitEvent += P2UpdateHit;
        p2.hitEvent += P1UpdateHit;
        p1.blockEvent += P2UpdateHit;
        p2.blockEvent += P1UpdateHit;
        p1.knockdownEvent += P2UpdateHit;
        p2.knockdownEvent += P1UpdateHit;
        p1.frameDataEvent += UpdateFrameData;
        p2.frameDataEvent += UpdateFrameData;
    }

    void UpdateHit()
    {
        frame = p1.minusFrames - p2.minusFrames;
        cancelFrame = p1.cancelMinusFrames;
        overlay1.UpdateStartup();
        overlay2.UpdateStartup();
        overlay1.UpdateAdvantage(frame, frame + cancelFrame);
        overlay2.UpdateAdvantage(-frame, -frame - cancelFrame);
    }

    void P1UpdateHit()
    {
        frame = p1.minusFrames - p2.minusFrames;
        cancelFrame = p1.cancelMinusFrames;
        overlay1.UpdateStartup();
        overlay1.UpdateAdvantage(frame, frame + cancelFrame);
        overlay2.UpdateAdvantage(-frame, -frame - cancelFrame);
    }

    void P2UpdateHit()
    {
        frame = p1.minusFrames - p2.minusFrames;
        cancelFrame = p1.cancelMinusFrames;
        overlay2.UpdateStartup();
        overlay1.UpdateAdvantage(frame, frame + cancelFrame);
        overlay2.UpdateAdvantage(-frame, -frame - cancelFrame);
    }



    // Update is called once per frame
    public void UpdateFrameData()
    {
        frame = p1.minusFrames - p2.minusFrames;
        cancelFrame = 0;
        overlay1.UpdateAdvantage(frame, frame + cancelFrame);
        overlay2.UpdateAdvantage(-frame, -frame - cancelFrame);
    }
}
