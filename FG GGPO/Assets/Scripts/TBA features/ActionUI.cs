using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image image;
    public Color col;
    Color defaultColor;
    // Start is called before the first frame update
    void Start()
    {
        defaultColor = image.color;
    }

    public void Setup(Move m)
    {
        if (m == null) text.text = "Jump";
        else
            text.text = m.moveInput;
    }

    public void Performed()
    {
        image.color = col;
    }

    public void ResetUI()
    {
        image.color = defaultColor;
    }
}
