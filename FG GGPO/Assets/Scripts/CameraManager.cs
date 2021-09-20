﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public CinemachineVirtualCamera[] cameras;

    public CinemachineVirtualCamera leftCamera;
    public CinemachineVirtualCamera rightCamera;
    public bool canSwitchRight;
    public bool isRightCamera;
    CinemachineBasicMultiChannelPerlin[] noises;
    [SerializeField] private float shakeTimer;
    private float startTimer;
    private float startIntensity;
    public bool toggle;
    public float toggleTimer;
    public float toggleCounter;

    public bool canCrossUp;

    InputHandler input1;
    InputHandler input2;
    Transform p1;
    Transform p2;



    public GameObject cam1;
    public GameObject cam2;

    public CameraController cc1;
    public CameraController cc2;

    [SerializeField] float dist1;
    [SerializeField] float dist2;
    public float deadZone;
    public Camera mainCamera;


    public float p1Y;
    public float p2Y;
    public float heightMod = 1f;
    public float modSmooth = 1f;
    float startZOffset;
    float refVelocity;
    CinemachineTransposer camTransposer;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        p1 = GameHandler.Instance.p1Transform;
        p2 = GameHandler.Instance.p2Transform;
        input1 = InputManager.Instance.p1Input;
        input2 = InputManager.Instance.p2Input;
        cc1.target = p1;
        cc1.lookTarget = p2;
        cc2.target = p2;
        cc2.lookTarget = p1;
        camTransposer = cameras[1].GetCinemachineComponent<CinemachineTransposer>();
        startZOffset = cameras[1].GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z;
    }

    private void FixedUpdate()
    {

        p1Y = GameHandler.Instance.p1Transform.position.y;
        p2Y = GameHandler.Instance.p2Transform.position.y;

        float modLerp = Mathf.SmoothDamp(camTransposer.m_FollowOffset.z, startZOffset - p1Y * heightMod, ref refVelocity, modSmooth);


        camTransposer.m_FollowOffset.z = modLerp;

        Vector3 v1 = new Vector3(p1.position.x, 0, p1.position.z);
        Vector3 v2 = new Vector3(p2.position.x, 0, p2.position.z);

        dist1 = Vector3.Distance(mainCamera.transform.position, v1);
        dist2 = Vector3.Distance(mainCamera.transform.position, v2);

        toggleCounter++;



        if (canSwitchRight)
        {
            if (mainCamera.WorldToViewportPoint(cc1.target.position).x > mainCamera.WorldToViewportPoint(cc2.target.position).x && !isRightCamera)
            {
                isRightCamera = true;
                leftCamera.Priority = 9;
                rightCamera.Priority = 10;
            }
            else if (mainCamera.WorldToViewportPoint(cc1.target.position).x < mainCamera.WorldToViewportPoint(cc2.target.position).x && isRightCamera)
            {
                isRightCamera = false;
                leftCamera.Priority = 10;
                rightCamera.Priority = 9;
            }
        }


        if (toggleCounter < toggleTimer || !canCrossUp) return;
        if (dist1 < dist2 + deadZone && toggle)
        {
            FlipCamera();
            toggleCounter = 0;
            input1.id = 1;
            input2.id = 2;


        }
        else if (dist1 + deadZone > dist2 && !toggle)
        {
            FlipCamera();
            toggleCounter = 0;
            input1.id = 2;
            input2.id = 1;
        }
    }
    [Button]
    public void FlipCamera()
    {
        print("flip cam");
        float dist1 = Vector3.Distance(mainCamera.transform.position, p1.position);
        float dist2 = Vector3.Distance(mainCamera.transform.position, p2.position);

        if (toggle)
        {
            toggle = false;
            cc1.target = p1;
            cc1.lookTarget = p2;
            cc2.target = p2;
            cc2.lookTarget = p1;
        }
        else
        {
            toggle = true;
            cc1.target = p2;
            cc1.lookTarget = p1;
            cc2.target = p1;
            cc2.lookTarget = p2;
        }
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

