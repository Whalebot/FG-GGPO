﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ScrollViewScript : MonoBehaviour
{
    [Range(0, 1)]
    public float scrollValue;
    public Scrollbar bar;
    public Transform contentContainer;
    public int step;
    public int top, bot;
    ScrollRect scroll;

    public void Awake()
    {


    }

    private void Start()
    {
        bar.numberOfSteps = GetChildrenInContainer() - 6;
        ResetScroll();
    }

    int GetChildrenInContainer()
    {
        int count = 0;
        foreach (Transform child in contentContainer)
        {
            if (child.gameObject.activeSelf)
                count++;
        }
 
        return count;
    }
    public void Selected(Transform t)
    {
       // GetChildrenInContainer();
        step = t.GetSiblingIndex();
        if (step > bot) {
            bot = step;
            top = bot - 5;
        }
        if (step < top) {
            top = step;
            bot = top + 5;
        }
        scrollValue = 1 - ((1 / (float)bar.numberOfSteps) * Mathf.Clamp((bot - 5), 0, bar.numberOfSteps));
        bar.value = scrollValue;
    }

    private void OnValidate()
    {
        //scrollValue = 1 - ((1 / (float)bar.numberOfSteps) * Mathf.Clamp((step - 5), 0, bar.numberOfSteps));
        //bar.value = scrollValue;
    }

    private void OnEnable()
    {
        // = GetComponent<ScrollRect>();
        ResetScroll();
    }

    private void OnDisable()
    {
        ResetScroll();
    }

    public void ResetScroll()
    {
        scrollValue = 1;
        //scroll.verticalNormalizedPosition = 1;
    }

    [Button]
    public void UpdateScroll()
    {
        bar.value = scrollValue;
        //if (down) scrollValue = Mathf.Clamp(scrollValue - (1F / (bar.numberOfSteps - 1)), 0, 1);
        //else
        //    scrollValue = Mathf.Clamp(scrollValue + (1F / (bar.numberOfSteps - 1)), 0, 1);
        //scroll.verticalNormalizedPosition = scrollValue;
    }
}
