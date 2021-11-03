using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UI.Controller;

public class PlayerNavigation : Controller
{
    [SerializeField] Button up,right,down,left;
    private void OnEnable() {
        base.inputManager.Move+=OnMove;
    }
    private void OnDisable() {
        base.inputManager.Move -= OnMove;
    }
    private new void OnMove(InputAction.CallbackContext context){
        float x=context.ReadValue<Vector2>().x;
        if(x==1)right.onClick?.Invoke();
        else if(x==-1)left.onClick?.Invoke();
        float y = context.ReadValue<Vector2>().y;
        if (y == 1) up.onClick?.Invoke();
        else if (y == -1) down.onClick?.Invoke();
    }
}