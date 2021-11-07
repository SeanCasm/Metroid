using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeySet : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] int compositeIndex = -1, compositeIndexGP = -1;
    //[SerializeField] InputActionReference actionReference;
    [SerializeField] string actionName;
    //[SerializeField] RebindKeys rebindKeys;
    private string actionTextAux;
    private const string text = "Waiting for input...";
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private InputAction action;
    private int index;
    private bool binding;
    private Text keyText, actionText;
    public void Start()
    {
        actionText = gameObject.GetChild(0).GetComponent<Text>();
        keyText = gameObject.GetChild(1).GetComponent<Text>();
        if (RebindKeys.deviceType ==CurrentDevice.Keyboard) index = compositeIndex;
        else index = compositeIndexGP;

        action = inputManager.GetActionByName(actionName);
        keyText.text = BindToText();
    }
    private void OnDisable() {
        if(binding){
            rebindingOperation.Cancel();
        }
    }
    public void SetIndex(bool keyboard){
        if(keyboard)index = compositeIndex;
        else index = compositeIndexGP;
    }
    public void SetKeyText(){
        keyText = gameObject.GetChild(1).GetComponent<Text>();
        keyText.text=BindToText();
    }
     
    public void StartRebindind()
    {
        if(!binding){
            actionTextAux = actionText.text;
            actionText.text = text;
            //rebindKeys.Input.SwitchCurrentActionMap("NNN");
            PerformIR();
        }
    }
    private string BindToText()
    {
        return InputControlPath.ToHumanReadableString(
            action.bindings[index].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    }
    public void PerformIR()
    {
        binding = true;
        action.Disable();
        #if UNITY_STANDALONE
        rebindingOperation = action.PerformInteractiveRebinding()
        .WithControlsExcluding("Mouse").
        OnMatchWaitForAnother(0.1f).
        OnCancel(op => RebindCancel()).
        OnComplete(operation => RebindComplete()).
        Start();
        #endif
        #if UNITY_ANDROID
        rebindingOperation = action.PerformInteractiveRebinding()
        .WithControlsExcluding("Keyboard").
        WithControlsExcluding("Mouse").
        OnMatchWaitForAnother(0.1f).
        OnCancel(op => RebindCancel()).
        OnComplete(operation => RebindComplete()).
        Start();
        #endif
         
    }
    private void RebindCancel()
    {
        binding = false;
        action.Enable();
        keyText.text = BindToText();
        //rebindKeys.Input.SwitchCurrentActionMap("Player");
        rebindingOperation.Dispose();
        actionText.text = actionTextAux;
    }
    private void RebindComplete()
    {
        binding = false;
        action.Enable();
        keyText.text = BindToText();
        //rebindKeys.Input.SwitchCurrentActionMap("Player");
        rebindingOperation.Dispose();
        actionText.text = actionTextAux;
        switch (actionText.text)
        {
            case "Select":
                UIInputKeyHandler.current.SetInputKeysSelect(keyText.text);
                break;
            case "Back":
                UIInputKeyHandler.current.SetInputKeyBack(keyText.text);
                break;
        }
    }
}