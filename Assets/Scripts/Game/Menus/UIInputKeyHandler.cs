using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputKeyHandler : MonoBehaviour
{
    public static UIInputKeyHandler current;
    [SerializeField] MenuKeys menuKeys;
    [System.Serializable]
    public struct MenuKeys
    {
        public TextMeshProUGUI back;
        public TextMeshProUGUI select;

    }
    private void Awake() {
        if(current==null)current=this;
    }
    private void OnEnable() {
        Pause.OnPauseInput+=Hide;
    }
    private void OnDisable() {
        Pause.OnPauseInput -= Hide;
    }
    private void Hide(bool hide){
        menuKeys.back.gameObject.SetActive(hide);
        menuKeys.select.gameObject.SetActive(hide);
    }
    public void SetInputKeyBack(string inputText){
        menuKeys.back.text="[" + inputText + "] Select";
    }
    public void SetInputKeysSelect(string inputText)
    {
        menuKeys.select.text = "[" + inputText + "] Back";
    }
    public void SetInputKeys(string back,string select)
    {
        menuKeys.back.text ="[" + back + "] Back";
        menuKeys.select.text = "[" + select + "] Select";
    }
}
