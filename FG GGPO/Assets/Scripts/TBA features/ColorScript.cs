using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ColorScript : MonoBehaviour
{
    public Renderer rend;
    public Material[] mats;
    public Color col;
    public Transform main;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        mats = rend.materials;
        if (GameHandler.Instance.isMirror && !GameHandler.Instance.IsPlayer1(main)) SetColor();

    }
    [Button]
    public void GetMaterials()
    {
        rend = GetComponent<Renderer>();
    }
    [Button]
    public void SetColor()
    {
        foreach (var item in mats)
        {
            item.SetColor("_MainColor", col);
        }
    }
}
