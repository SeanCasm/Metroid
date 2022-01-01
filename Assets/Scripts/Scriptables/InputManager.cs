using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using static Metroid;

//[CreateAssetMenu(fileName = "InputManager", menuName = "ScriptableObjects/Input/InputManager")]
public class InputManager : MonoBehaviour, IPlayerActions, IUIActions
{
    [SerializeField] TextMeshProUGUI controlPanel;
    [SerializeField] Button inputButton;
    [SerializeField] GameObject options, control, touchpad, selectBack;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] List<KeySet> keyCollection = new List<KeySet>();
    public PlayerInput playerInput;
    private Metroid inputClass;
    public Action<InputAction.CallbackContext> HorizontalMovement, HorizontalMovementCanceled = delegate { };
    public Action<InputAction.CallbackContext> VerticalMovement = delegate { }, VerticalMovementCanceled = delegate { };
    public Action<InputAction.CallbackContext> Jump = delegate { }, JumpCanceled = delegate { };
    public Action<InputAction.CallbackContext> Run = delegate { }, RunCanceled = delegate { };
    public Action<InputAction.CallbackContext> Fire = delegate { }, FireCanceled = delegate { }, FirePerformed = delegate { };
    public Action<InputAction.CallbackContext> Morph = delegate { };
    public Action<InputAction.CallbackContext> AmmoSelection = delegate { };
    public Action<InputAction.CallbackContext> AimAngleUp = delegate { }, AimAngleUpCanceled = delegate { };
    public Action<InputAction.CallbackContext> AimAngleDown = delegate { }, AimAngleDownCanceled = delegate { };
    public Action<InputAction.CallbackContext> Submit = delegate { };
    public Action<InputAction.CallbackContext> Pause = delegate { }, PlayerPause = delegate { };
    public Action<InputAction.CallbackContext> Back = delegate { };
    public Action<InputAction.CallbackContext> Select = delegate { };
    public Action<InputAction.CallbackContext> Move = delegate { }, MoveCanceled = delegate { };
    public InputAction moveAction { get; private set; }
    public InputAction playerAD { get; private set; }
    private Dictionary<byte, Action> canceledUnscaledActions = new Dictionary<byte, Action>();
    private Queue inputQueue;
    public bool lockFireInput { get; set; }
    public bool isTouchingStick { get; set; }

    private CurrentDevice actualDevice;
    private void Start()
    {
#if UNITY_ANDROID
        inputButton.interactable=false;
        controlPanel.text = "Gamepad options";
#endif
        CurrentControlScheme(playerInput);
    }
    public void CurrentControlScheme(PlayerInput playerInput)
    {
        switch (playerInput.currentControlScheme)
        {
            case "Gamepad":
#if UNITY_STANDALONE

                RebindKeys.deviceType = CurrentDevice.Gamepad;
                actualDevice = CurrentDevice.Gamepad;
                controlPanel.text = "Gamepad options";
                UIInputKeyHandler.current.SetInputKeys(InputControlPath.ToHumanReadableString(
             GetBackEffectivePath(1),
             InputControlPath.HumanReadableStringOptions.OmitDevice
         ), InputControlPath.ToHumanReadableString(GetSelectEffectivePath(1),
             InputControlPath.HumanReadableStringOptions.OmitDevice));
                keyCollection.ForEach(item =>
                   {
                       item.Start();
                       item.SetIndex(false);
                       item.SetKeyText();
                   });
#endif

#if UNITY_ANDROID
                //touchpad.SetActive(false);
                selectBack.SetActive(true);
                inputButton.interactable = true;

#endif

                break;
#if UNITY_STANDALONE
            case "Keyboard&Mouse":
                RebindKeys.deviceType = CurrentDevice.Keyboard;
                actualDevice = CurrentDevice.Keyboard;
                controlPanel.text = "Keyboard options";
                keyCollection.ForEach(item =>
                {
                    item.Start();
                    item.SetIndex(true);
                    item.SetKeyText();

                });
                UIInputKeyHandler.current.SetInputKeys(InputControlPath.ToHumanReadableString(
            GetBackEffectivePath(0),
            InputControlPath.HumanReadableStringOptions.OmitDevice
        ), InputControlPath.ToHumanReadableString(GetSelectEffectivePath(0),
            InputControlPath.HumanReadableStringOptions.OmitDevice));

                break;
#endif
#if UNITY_ANDROID
            case "Keyboard&Mouse":
                if (RebindKeys.isEnable)
                {
                    control.SetActive(false);
                    options.SetActive(true);
                    eventSystem.SetSelectedGameObject(options.GetChild(0));
                }
                inputButton.interactable = false;
                touchpad.SetActive(true);
                actualDevice = CurrentDevice.Touch;
                selectBack.SetActive(false);
                break;
#endif
        }
    }

    private void OnEnable()
    {
        playerInput.onControlsChanged += CurrentControlScheme;
    }
    private void OnDisable()
    {
        playerInput.onControlsChanged -= CurrentControlScheme;
    }
    private void Awake()
    {
        inputClass = new Metroid();
        inputClass.Player.SetCallbacks(this);
        inputClass.UI.SetCallbacks(this);
        inputClass.UI.Enable();
        playerAD = inputClass.Player.AD;
        moveAction = inputClass.UI.Navigate;
    }
    public void AddToUnscaledActions(byte id, Action action) => canceledUnscaledActions[id] = action;
    public void InvokeUnscaledActions()
    {
        foreach (var e in canceledUnscaledActions)
        {
            e.Value.Invoke();
        }
        canceledUnscaledActions.Clear();
    }
    public void LoadFromJson(string r) => inputClass.LoadBindingOverridesFromJson(r);
    public void EnablePlayerInput() => inputClass.Player.Enable();
    public void DisableUIInput() => inputClass.UI.Disable();
    public void EnableUIInput() => inputClass.UI.Enable();
    public void DisablePlayerInput() => inputClass.Player.Disable();
    public void DisableFireInput() => inputClass.Player.Fire.Disable();
    public void EnableFireInput() => inputClass.Player.Fire.Enable();
    public void DisableAll()
    {
        inputClass.Player.Disable();
        inputClass.UI.Disable();
    }
    public void EnableAll()
    {
        inputClass.Player.Enable();
        inputClass.UI.Enable();
    }
    public string GetBackEffectivePath(byte index)
    {
        return inputClass.UI.Cancel.bindings[index].effectivePath;
    }
    public string GetSelectEffectivePath(byte index)
    {
        return inputClass.UI.Submit.bindings[index].effectivePath;
    }
    public InputAction GetActionByName(string name)
    {
        return inputClass.FindAction(name);
    }
    public void SaveBindingsToJson()
    {
        PlayerPrefs.SetString("rebinds", inputClass.SaveBindingOverridesAsJson());
    }
    public void SetAllInput(bool active)
    {
        if (active) inputClass.Player.Enable();
        else inputClass.Player.Disable();
    }
    public void DisablePlayerControls()
    {
        inputClass.Player.Morphball.Disable();
        inputClass.Player.Tab.Disable();
        inputClass.Player.Fire.Disable();
    }
    //Called from UnityEvent
    public void EnablePlayerControls()
    {
        inputClass.Player.Morphball.Enable();
        inputClass.Player.Tab.Enable();
    }


    public void OnAimdown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            AimAngleDown.Invoke(context);
        if (context.phase == InputActionPhase.Canceled)
            AimAngleDownCanceled.Invoke(context);
    }

    public void OnAimup(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            AimAngleUp.Invoke(context);
        if (context.phase == InputActionPhase.Canceled)
            AimAngleUpCanceled.Invoke(context);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Back.Invoke(context);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Fire(context);
        if (context.phase == InputActionPhase.Performed)
            FirePerformed(context);
        if (context.phase == InputActionPhase.Canceled)
            FireCanceled(context);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Jump(context);
        if (context.phase == InputActionPhase.Canceled)
            JumpCanceled(context);
    }

    public void OnMainpause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Pause.Invoke(context);
    }

    public void OnMorphball(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Morph.Invoke(context);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Move.Invoke(context);
        if (context.phase == InputActionPhase.Canceled)
            MoveCanceled.Invoke(context);
    }

    public void OnPlayermenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            PlayerPause.Invoke(context);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Run.Invoke(context);
        if (context.phase == InputActionPhase.Canceled)
            RunCanceled.Invoke(context);
    }

    public void OnTab(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            AmmoSelection.Invoke(context);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        Submit.Invoke(context);
    }
    public void OnAD(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            HorizontalMovement?.Invoke(context);
        if (context.phase == InputActionPhase.Canceled)
            HorizontalMovementCanceled.Invoke(context);
    }
    public void OnWS(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            VerticalMovement.Invoke(context);
        if (context.phase == InputActionPhase.Canceled)
            VerticalMovementCanceled.Invoke(context);
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Select.Invoke(context);
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
    }
}
public enum CurrentDevice { Gamepad, Keyboard, Touch }
