using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SaveMenu : MonoBehaviour
{
    [SerializeField]GameObject saveMenuPrefab,saveTextObject;
    [SerializeField]Transform canvas;
    private Button saveYes,saveNo;
    private GameObject saveMenu;
    private SaveStation currentST;
    private void OnEnable() {
        GameEvents.save+=HandleSavePanel;
        GameEvents.saveMessage+=HandleAcceptSave;
    }
    private void OnDisable() {
        GameEvents.save -= HandleSavePanel;
        GameEvents.saveMessage -= HandleAcceptSave;
    }
    private void HandleSavePanel(SaveStation currentST)
    {
        this.currentST = currentST;
        saveMenu = Instantiate(saveMenuPrefab, canvas.position, Quaternion.identity, canvas);
        saveYes=saveMenu.GetChild(0).GetComponent<Button>();//yes button
        EventSystem.current.SetSelectedGameObject(saveYes.gameObject);
        saveNo= saveMenu.GetChild(1).GetComponent<Button>();//no button
        saveYes.onClick.AddListener(()=>SetOption(true));
        saveNo.onClick.AddListener(()=>SetOption(false));
    }
    /// <summary>
    /// Set the option selected in the save panel UI to the SaveStation.
    /// </summary>
    /// <param name="choise">true: save game, false: dont save.</param>
    public void SetOption(bool choise)
    {
        currentST.saveGame(choise);
        Destroy(saveMenu);
    }
    private void HandleAcceptSave()
    {
        saveTextObject.SetActive(true);
        Invoke("HideMsg",2.5f);
    }
    private void HideMsg(){
        saveTextObject.SetActive(false);
    }
}
