using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollObject : MonoBehaviour
{
    public ScrollViewScript scrollViewScript;
    // Start is called before the first frame update
    void Start()
    {
        scrollViewScript = GetComponentInParent<ScrollViewScript>();
    }

    // Update is called once per frame
    public void Selected()
    {
        scrollViewScript.Selected(transform);
    }
}
