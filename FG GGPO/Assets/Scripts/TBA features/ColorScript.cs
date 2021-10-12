using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorScript : MonoBehaviour
{
    public Renderer rend;
    public Material mat;
    public Color col;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;

    }

    // Update is called once per frame
    void Update()
    {
        mat.SetColor("_MainColor", col);
    }
}
