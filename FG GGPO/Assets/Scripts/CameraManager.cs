using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    public CinemachineVirtualCamera[] cameras;
    CinemachineBasicMultiChannelPerlin[] noises;
    [SerializeField] private float shakeTimer;
    private float startTimer;
    private float startIntensity;
    public bool toggle;
    public float toggleTimer;
    public float toggleCounter;

    public Transform defaultTarget;

    public InputHandler input1;
    public InputHandler input2;
    public Transform p1;
    public Transform p2;

    public GameObject cam1;
    public GameObject cam2;

    public CameraController cc1;
    public CameraController cc2;

    [SerializeField]float dist1;
    [SerializeField] float dist2;
    public float deadZone;
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
        Vector3 v1 = new Vector3(p1.position.x, 0, p1.position.z);
        Vector3 v2 = new Vector3(p2.position.x, 0, p2.position.z);
        dist1 = Vector3.Distance(mainCamera.transform.position, v1);
        dist2 = Vector3.Distance(mainCamera.transform.position, p2.position);

        toggleCounter++;
        if (toggleCounter < toggleTimer) return;
        if (dist1 < dist2 + deadZone && toggle)
        {
            FlipCamera();
            toggleCounter = 0;
            //toggle = false;
            input1.id = 1;
            input2.id = 2;
            //cam1.SetActive(true);
            //cam2.SetActive(false);

        }
        else if (dist1 + deadZone > dist2 && !toggle)
        {
            FlipCamera();
            toggleCounter = 0;
            //toggle = true;
            input1.id = 2;
            input2.id = 1;
            //cam1.SetActive(false);
            //cam2.SetActive(true);
        }
    }
    [Button]
    public void FlipCamera()
    {
        float dist1 = Vector3.Distance(mainCamera.transform.position, p1.position);
        float dist2 = Vector3.Distance(mainCamera.transform.position, p2.position);

        if (toggle)
        {
            toggle = false;
            cc1.target = p1;
            cc1.lookTarget = p2;
            cc2.target = p2;
            cc2.lookTarget = p1;
            //input1.id = 1;
            //input2.id = 2;
            //cam1.SetActive(true);
            //cam2.SetActive(false);
        }
        else
        {
            toggle = true;
            cc1.target = p2;
            cc1.lookTarget = p1;
            cc2.target = p1;
            cc2.lookTarget = p2;
            //input1.id = 2;
            //input2.id = 1;
            //cam1.SetActive(false);
            //cam2.SetActive(true);
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
