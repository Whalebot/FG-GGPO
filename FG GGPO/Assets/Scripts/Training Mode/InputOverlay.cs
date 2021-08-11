using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputOverlay : MonoBehaviour
{
    public InputHandler inputHandler;
    public Transform marker;
    public GameObject panel;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;
    public GameObject arrow1;
    public GameObject arrow2;
    public GameObject arrow3;
    public GameObject arrow4;
    public GameObject arrow6;
    public GameObject arrow7;
    public GameObject arrow8;
    public GameObject arrow9;
    public GameObject arrowBlank;
    public GameObject buttons;
    public GameObject frameCountPrefab;
    public int frameCount;
    public TextMeshProUGUI frameCountText;
    public int maxObjects;
    public int currentObjects;
    int lastDirectional;
    // public List<int> directionals;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameHandler.isPaused) return;

        marker.localPosition = new Vector3(50 * inputHandler.inputDirection.x, 50 * inputHandler.inputDirection.y, 0);
        if (inputHandler.updatedDirectionals || inputHandler.updatedButtons)
        {
            currentObjects++;
            DirectionalInput(); ButtonInput(); InputDuration(); FrameTime();
            if (maxObjects < currentObjects) { CleanButtonHistory(); }
        }

        frameCount++;
        frameCountText.text = "" + frameCount;
    }

    void CleanButtonHistory()
    {
        currentObjects--;
        Destroy(panel.transform.GetChild(panel.transform.childCount - 1).gameObject);

        Destroy(panel2.transform.GetChild(panel2.transform.childCount - 1).gameObject);
        Destroy(panel3.transform.GetChild(panel3.transform.childCount - 1).gameObject);
        Destroy(panel4.transform.GetChild(panel4.transform.childCount - 1).gameObject);

    }


    void ClearButtonHistory()
    {
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in panel2.transform)
        {
            Destroy(child.gameObject);
        }
    }


    void InputDuration()
    {
        GameObject temp = Instantiate(frameCountPrefab, panel3.transform);
        temp.transform.SetSiblingIndex(0);
        frameCountText = temp.GetComponent<TextMeshProUGUI>();
        frameCount = 0;
    }

    void FrameTime()
    {
        GameObject temp = Instantiate(frameCountPrefab, panel4.transform);
        temp.transform.SetSiblingIndex(0);
        temp.GetComponent<TextMeshProUGUI>().text = "" + GameHandler.Instance.gameFrameCount;

    }

    void ButtonInput()
    {
        inputHandler.updatedButtons = false;
        if (!inputHandler.netButtons[0] && !inputHandler.netButtons[1] && !inputHandler.netButtons[2] && !inputHandler.netButtons[3] && !inputHandler.netButtons[4] && !inputHandler.netButtons[5])
        {
            Instantiate(arrowBlank, panel2.transform).transform.SetSiblingIndex(0);
        }
        else
        {
            GameObject temp = Instantiate(buttons, panel2.transform);
            temp.transform.SetSiblingIndex(0);
            if (inputHandler.netButtons[0]) temp.transform.GetChild(0).gameObject.SetActive(true);
            if (inputHandler.netButtons[1]) temp.transform.GetChild(1).gameObject.SetActive(true);
            if (inputHandler.netButtons[2]) temp.transform.GetChild(2).gameObject.SetActive(true);
            if (inputHandler.netButtons[3]) temp.transform.GetChild(3).gameObject.SetActive(true);
            if (inputHandler.netButtons[4]) temp.transform.GetChild(6).gameObject.SetActive(true);
            if (inputHandler.netButtons[5]) temp.transform.GetChild(4).gameObject.SetActive(true);
            //if (inputHandler.netButtons[6]) temp.transform.GetChild(7).gameObject.SetActive(true);
            //if (inputHandler.netButtons[7]) temp.transform.GetChild(5).gameObject.SetActive(true);
        }


    }

    void DirectionalInput()
    {
        inputHandler.updatedDirectionals = false;

        switch (inputHandler.directionals[inputHandler.directionals.Count - 1])
        {
            case 9:
                Instantiate(arrow9, panel.transform).transform.SetSiblingIndex(0);
                break;
            case 8:
                Instantiate(arrow8, panel.transform).transform.SetSiblingIndex(0);
                break;
            case 7:
                Instantiate(arrow7, panel.transform).transform.SetSiblingIndex(0);
                break;
            case 6:
                Instantiate(arrow6, panel.transform).transform.SetSiblingIndex(0);
                break;
            case 5:
                Instantiate(arrowBlank, panel.transform).transform.SetSiblingIndex(0);
                break;
            case 4:
                Instantiate(arrow4, panel.transform).transform.SetSiblingIndex(0);
                break;
            case 3:
                Instantiate(arrow3, panel.transform).transform.SetSiblingIndex(0);
                break;
            case 2:
                Instantiate(arrow2, panel.transform).transform.SetSiblingIndex(0);
                break;
            case 1:
                Instantiate(arrow1, panel.transform).transform.SetSiblingIndex(0);
                break;

        }
    }
}