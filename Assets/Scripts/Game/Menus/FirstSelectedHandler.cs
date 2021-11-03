using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FirstSelectedHandler:MonoBehaviour
{
    [SerializeField]Button gameVolumeSettings;
    [SerializeField] GameObject pauseSettings,pauseResume,mainSettings;
    [SerializeField] EventSystem eventSystem;
    private Button mainMenuFirst,slotButton,settingsButton,pauseFirst, retryFirst,continueFirst;
    public Button SettingsFirst{get=>gameVolumeSettings;}
    public Button MenuFirst{get=>mainMenuFirst;set=>mainMenuFirst=value;}
  
    public Button RetryFirst
    {
        get => retryFirst;
        set
        {
            retryFirst = value;
            if (retryFirst) SetGameObjectToEventSystem(retryFirst);
        }
    }
    public void SetMainMenuSettingsFirst(){
        eventSystem.SetSelectedGameObject(mainSettings);
    }
    public void SetPauseResumeFirst(){
        eventSystem.SetSelectedGameObject(pauseResume);
    }
    public void SetPauseSettingsFirst(){
        eventSystem.SetSelectedGameObject(pauseSettings);
    }
    public void SetGameObjectToEventSystem(Button first){
        eventSystem.SetSelectedGameObject(first.gameObject);
        eventSystem.firstSelectedGameObject=first.gameObject;
        if(first.gameObject.name=="Continue"){
            continueFirst=first;
        }
    }
    public void SetGameObjectToEventSystem(TMP_Dropdown dropdown){
        EventSystem.current.SetSelectedGameObject(dropdown.gameObject);
        EventSystem.current.firstSelectedGameObject = dropdown.gameObject;
    }
    public void SetGameObjectToEventSystem(Slider first)
    {
        EventSystem.current.SetSelectedGameObject(first.gameObject);
        EventSystem.current.firstSelectedGameObject = first.gameObject;
    }
    public void SetInteractableButtonSettings(Button button){
        button.interactable = !button.interactable;
        if(button.interactable)settingsButton=null;
        settingsButton = button;
    }
}