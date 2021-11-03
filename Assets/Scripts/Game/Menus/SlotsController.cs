using System.Collections;
using System.Collections.Generic;
using UI.Controller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class SlotsController : Controller
{
    [Header("Event System config")]
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject options;
    private Button slot;
    private GameObject curPanel; 
    private bool onPanel;
    void OnDisable(){
        base.inputManager.Back-=Back;
    }
    void OnEnable(){
        base.inputManager.Back += Back;
    }
    new void Back(InputAction.CallbackContext context) {
        if(onPanel){
            this.slot.interactable = !this.slot.interactable;
            MenuPointer.canMove = true;
            curPanel.GetParent().SetActive(false);
            this.curPanel =null;
            eventSystem.SetSelectedGameObject(slot.gameObject);
            onPanel = false;
        }
    }
    public void SetCurrentSlotPressed(Button slot){
        this.slot=slot;
        this.slot.interactable = !this.slot.interactable;
        MenuPointer.canMove = this.slot.interactable;
        onPanel=true;
    }
    public void SetCurrentPanelFirstSelected(GameObject panelFirst){
        curPanel=panelFirst;
        eventSystem.SetSelectedGameObject(panelFirst);
        onPanel=true;
    }
    public void SetOptionsFirstSelected(){
        eventSystem.SetSelectedGameObject(options);
    }
}