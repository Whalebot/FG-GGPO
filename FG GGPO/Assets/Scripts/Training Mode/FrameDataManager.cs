using System.Collections;
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
        p1.frameDataEvent += UpdateFrameData;
        p2.frameDataEvent += UpdateFrameData;
    }

    // Update is called once per frame
    void UpdateFrameData()
    {
        frame = p1.minusFrames - p2.minusFrames;
        overlay1.UpdateAdvantage(frame);
        overlay2.UpdateAdvantage(-frame);
    }
}
