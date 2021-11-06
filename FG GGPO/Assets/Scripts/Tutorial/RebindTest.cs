using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class RebindTest : MonoBehaviour
{
    public InputActionReference inputActionReference;
    public bool p1;
    public bool excludeMouse = true;
    public int selectedBinding;
    public InputBinding.DisplayStringOptions displayStringOptions;
    public InputBinding inputBinding;
    public int bindingIndex;
    public string actionName;

    public TextMeshProUGUI actionText;
    public Button rebindButton;
    public TextMeshProUGUI rebindText;
    public Button resetButton;

    void Start()
    {
        rebindButton.onClick.AddListener(() => DoBind());
        resetButton.onClick.AddListener(() => ResetBinding());

        if (inputActionReference != null)
        {
            if (p1)
                InputManager.LoadBindingOverrideP1(actionName);
            else
                InputManager.LoadBindingOverrideP2(actionName);
            GetBindingInfo();
            UpdateUI();
        }
        if (p1)
        {
            InputManager.Instance.p1Input.rebindComplete += UpdateUI;
            InputManager.Instance.p1Input.rebindCancelled += UpdateUI;
        }
        else
        {
            InputManager.Instance.p2Input.rebindComplete += UpdateUI;
            InputManager.Instance.p2Input.rebindCancelled += UpdateUI;
        }
        //var ui = GetComponent<RebindActionUI>();
        //var currentReference = ui.actionReference;
        //var actionName = currentReference.action.name;
        //ui.actionReference = InputActionReference.Create(m_Actions[actionName]);
    }

    private void OnDisable()
    {
        if (p1)
        {
            InputManager.Instance.p1Input.rebindComplete -= UpdateUI;
            InputManager.Instance.p1Input.rebindCancelled -= UpdateUI;
        }
        else
        {
            InputManager.Instance.p2Input.rebindComplete -= UpdateUI;
            InputManager.Instance.p2Input.rebindCancelled -= UpdateUI;
        }
    }

    private void DoBind()
    {
        if (p1)
        {
            InputManager.Instance.p1Input.StartRebind(actionName, bindingIndex, rebindText);
        }
        else
        {
            InputManager.Instance.p2Input.StartRebind(actionName, bindingIndex, rebindText);
        }
    }
    private void ResetBinding()
    {
        if (p1)
            InputManager.ResetBindingP1(actionName, bindingIndex);
        else
            InputManager.ResetBindingP2(actionName, bindingIndex);
        UpdateUI();
    }

    private void OnValidate()
    {
        if (inputActionReference == null) return;
        GetBindingInfo();
        UpdateUI();
    }

    private void GetBindingInfo()
    {
        if (inputActionReference.action != null)
        {
            actionName = inputActionReference.action.name;
        }
        if (inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }

    private void UpdateUI()
    {
        actionText.text = actionName;
        if (Application.isPlaying)
        {
            if (p1)
                rebindText.text = InputManager.GetBindingNameP1(actionName, bindingIndex);
            else rebindText.text = InputManager.GetBindingNameP2(actionName, bindingIndex);

        }
        else rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
    }
}
