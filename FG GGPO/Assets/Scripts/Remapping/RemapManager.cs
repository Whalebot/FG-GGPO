using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemapManager : MonoBehaviour
{
    public GameObject defaultButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        UIManager.Instance.SetActive(defaultButton);
    }
}
