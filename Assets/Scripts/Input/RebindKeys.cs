using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class RebindKeys : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    public static bool isEnable;
    public static CurrentDevice deviceType;
    private InputActionMap _inputActionMap;
  
    /// <summary>
    /// Saves the input bindings at button onClick.
    /// </summary>
    public void Save(){
        inputManager.SaveBindingsToJson();
    }
    private void OnEnable() {
        isEnable=true;
    }
    private void OnDisable() {
        isEnable=false;
    }
}