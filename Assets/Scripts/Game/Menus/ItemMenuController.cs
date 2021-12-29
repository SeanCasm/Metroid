using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UI.Controller;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemMenuController : Controller
{
    [SerializeField] EventSystem eventSystem;
    [SerializeField] UnityEvent backEvent;
    [SerializeField] ButtonUtilities buttonEssentials;
    [SerializeField] GameObject menuPointer;
    [SerializeField] Canvas hudCanvas;
    int idFirst;
    Button firstItem;
    private MenuPointer mPointer;
    void Awake()
    {
        mPointer = menuPointer.GetComponent<MenuPointer>();
    }

    void OnEnable()
    {
        StartCoroutine("SelectContinueButtonLater");
        base.inputManager.Back+=Back;
        firstItem.Select();
        hudCanvas.overrideSorting=true;
        hudCanvas.sortingOrder=8;
    }
    void OnDisable()
    {
        base.inputManager.Back -= Back;
        menuPointer.SetActive(false);
        hudCanvas.overrideSorting=false;
    }
    IEnumerator SelectContinueButtonLater()
    {
        yield return null;
        eventSystem.SetSelectedGameObject(null);
        menuPointer.SetActive(true);
        eventSystem.SetSelectedGameObject(firstItem.gameObject);
    }
    public void Back(InputAction.CallbackContext context){
        backEvent.Invoke();
    }
    /// <summary>
    /// Sets the navigation to all buttons in item menu UI.
    /// </summary>
    public void SetButtonNavigation()
    {
        foreach(var e in buttonEssentials.buttons){
            if(e.Value.gameObject.activeSelf){
                firstItem = e.Value;
                idFirst = e.Value.GetComponent<ButtonID>().ID;
                break;
            }
        }
         
    }
}