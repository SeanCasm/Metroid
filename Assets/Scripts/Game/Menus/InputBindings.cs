using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class InputBindings : MonoBehaviour
{
    [SerializeField]
    Text controlText;
    PlayerInput myInput;
    InputAction playerAction;
    void Awake()
    {
        myInput = GetComponent<PlayerInput>();
    }
    public void SetTextField()
    {
    }
    public void RebindKey(InputAction actionToRebind)
    {
        var rebindOperation = actionToRebind.PerformInteractiveRebinding()
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.1f)
                    .Start();
    }
}
