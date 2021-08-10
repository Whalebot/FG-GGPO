using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public CinemachineVirtualCamera[] cameras;
    CinemachineBasicMultiChannelPerlin[] noises;
    [SerializeField] private float shakeTimer;
    private float startTimer;
    private float startIntensity;
    public bool toggle;

    public CinemachineTargetGroup targetGroup;
    public CinemachineVirtualCamera groupCamera;

    public Transform defaultTarget;

    public InputHandler input1;
    public InputHandler input2;
    public Transform p1;
    public Transform p2;

    public Camera mainCamera;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //noises = new CinemachineBasicMultiChannelPerlin[cameras.Length];
        //for (int i = 0; i < noises.Length; i++)
        //{
        //    noises[i] = cameras[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //}

        //ShakeCamera(0, 0.1F);
    }

    private void FixedUpdate()
    {
        float dist1 = Vector3.Distance(mainCamera.transform.position, p1.position);
        float dist2 = Vector3.Distance(mainCamera.transform.position, p2.position);

        if (dist1 < dist2)
        {
            input1.id = 1;
            input2.id = 2;
        }
        else
        {
            input1.id = 2;
            input2.id = 1;
        }
    }

    public void SetGroupTarget(Transform temp)
    {
        targetGroup.m_Targets[1].target = temp;
        // groupCamera.gameObject.SetActive(true);
        shakeTimer = 0;
        for (int i = 0; i < noises.Length; i++)
            noises[i].m_AmplitudeGain = 0;
    }

    public void SetGroupTarget()
    {
        groupCamera.gameObject.SetActive(true);
        shakeTimer = 0;
        for (int i = 0; i < noises.Length; i++)
            noises[i].m_AmplitudeGain = 0;
    }

    public void RevertCamera()
    {
        groupCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            {
                for (int i = 0; i < noises.Length; i++)
                {
                    noises[i].m_AmplitudeGain = Mathf.Lerp(startIntensity, 0f, (1 - (shakeTimer / startTimer)));

                } 
            }
        }
    }

    public void ShakeCamera(float intensity, float time)
    {
        //startIntensity = intensity;
        //shakeTimer = time;
        //startTimer = time;
    }
}
