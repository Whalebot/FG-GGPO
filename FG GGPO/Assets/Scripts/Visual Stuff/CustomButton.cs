using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public class CustomButton : MonoBehaviour
{
    public bool selected;
    public Color defaultColor;
    public Color selectedColor;
    public float startWidth, startHeight;
    public float expandedWidth, expandedHeight;
    public Image image;
    public RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {

    }

    [Button]
    public void SaveValues()
    {
        if (!selected)
        {
            image = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
            defaultColor = image.color;
            startWidth = rect.sizeDelta.x;
            startHeight = rect.sizeDelta.y;
        }
    }
    [Button]
    public void ExpandTab()
    {
        if (selected)
        {
            image.color = selectedColor;
            rect.sizeDelta = new Vector2(expandedWidth, expandedHeight);
        }
        else
        {
            image.color = defaultColor;
            rect.sizeDelta = new Vector2(startWidth, startHeight);
        }
    }

}
