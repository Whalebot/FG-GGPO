using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class RemapManager : MonoBehaviour
{
    public GameObject defaultButton;
    public InputActionAsset tanksInputActions;
    private InputActionMap playerActionMap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Button]
    void DisableActionMap() {
        playerActionMap = tanksInputActions.FindActionMap("Default");
        playerActionMap.Disable();
    }
    [Button]
    void EnableActionMap()
    {
        playerActionMap.Enable();
    }



    private void OnEnable()
    {
     //   UIManager.Instance.SetActive(defaultButton);
    }
}
