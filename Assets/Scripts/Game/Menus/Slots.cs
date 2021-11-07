using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;

public class Slots : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] GameObject[] buttonGames, energySlots, missileSlots, superMissileSlots, superBombSlots,stickySlots;
    [SerializeField] Button[] eraseButtons;
    [SerializeField] RectTransform[] energyUISlots;
    [SerializeField] UnityEvent startGame;
    [SerializeField]SaveAndLoad saveLoad;
    [SerializeField] RectTransform pointer,canvas;
    [Tooltip("Array of play time in order: slot-1,slot-2,slot-3")]
    [SerializeField] TextMeshProUGUI[] times;
    private GameData data;
    private int slotToDelete;
    public static int slot;
    public static bool retry;
    // Start is called before the first frame update
    void Start()
    {
        Pause.OnPauseInput?.Invoke(true);
        Pause.onGame=false;
        string rebinds=PlayerPrefs.GetString("rebinds",string.Empty);
        if(!string.IsNullOrEmpty(rebinds))inputManager.LoadFromJson(rebinds);
        
        if (retry){
            SetData(slot);
            return;
        }
        LoadDataToSlots();
        SetInteractableToEraseButtons();
    }
    private void SetInteractableToEraseButtons(){
        for(int i=0;i<3;i++){
            eraseButtons[i].interactable = SaveSystem.FileExists(i);
        }
    }
    public void LoadDataToSlots()
    {
        for(int i=0;i<3;i++){
            if(SaveSystem.LoadPlayerSlot(i)!=null){
                data = SaveSystem.LoadPlayerSlot(i);
                ShowItems(missileSlots, energySlots, energyUISlots, superMissileSlots, superBombSlots, i);
            }
        }
        for(int i=0;i<3;i++)ShowButtons(i);
    }
    void ShowItems(GameObject[] missilesSlot,GameObject[] energySlot,RectTransform[] energyUI,
        GameObject[] superMissileSlot,GameObject[] superBombSlot,int slotIndex)
    {
        int totalTanks = data.tanks;
        
        if (data.ammoMunition.ContainsKey(0))
        {
            missilesSlot[slotIndex].SetActive(true);
            missilesSlot[slotIndex].GetComponentInChildren<TextMeshProUGUI>().text = data.ammoMunition[0].ToString();
        }
        if (data.ammoMunition.ContainsKey(1))
        {
            superMissileSlot[slotIndex].SetActive(true);
            superMissileSlot[slotIndex].GetComponentInChildren<TextMeshProUGUI>().text= data.ammoMunition[1].ToString();
        }
        if (data.ammoMunition.ContainsKey(2))
        {
            superBombSlot[slotIndex].SetActive(true);
            superBombSlot[slotIndex].GetComponentInChildren<TextMeshProUGUI>().text = data.ammoMunition[2].ToString();
        }
        if(data.ammoMunition.ContainsKey(3)){
            stickySlots[slotIndex].SetActive(true);
            stickySlots[slotIndex].GetComponentInChildren<TextMeshProUGUI>().text = data.ammoMunition[3].ToString();
        }
        times[slotIndex].text = TimeCounter.TimeArrayIntToString(data.time);
        energySlot[slotIndex].SetActive(true);
        energyUI[slotIndex].sizeDelta = new Vector2(16f * totalTanks, 16f);
    }
    //Called in onClick event from erase slot button
    public void ShowDeletePanel(int slotIndex)
    {
        slotToDelete=slotIndex;
    }
    //Called in the delete save panel
    public void DeleteGameSlotData(){
        SaveSystem.ErasePlayerSlot(slotToDelete);
        times[slotToDelete].text="00:00:00.00";
        missileSlots[slotToDelete].SetActive(false);
        superMissileSlots[slotToDelete].SetActive(false);
        superBombSlots[slotToDelete].SetActive(false);
        stickySlots[slotToDelete].SetActive(false);
        energyUISlots[slotToDelete].sizeDelta = new Vector2(0, 16f);
        energySlots[slotToDelete].SetActive(false);
        ShowButtons(slotToDelete);
    }
    public void ShowButtons(int slotIndex)
    {
        TextMeshProUGUI buttonText= buttonGames[slotIndex].GetComponentInChildren<TextMeshProUGUI>();
        if (SaveSystem.FileExists(slotIndex))buttonText.text="Continue";
        else buttonText.text = "New Game";
    }
    private void NewGameData()
    {
        SaveAndLoad.newGame = true;
        if(!retry) SceneHandler.current.LoadFromNewGame();
        else SceneHandler.current.LoadFromRetryScreen();
    }
    public void SetData(int loadSlot)
    {
        slot=loadSlot;
        SaveAndLoad.slot = loadSlot;
        if (SaveSystem.LoadPlayerSlot(loadSlot) != null) { 
            SaveAndLoad.newGame = false;
            saveLoad.LoadPlayerSlot(loadSlot); 
        }
        else NewGameData();
        startGame.Invoke();
        Pause.OnPauseInput?.Invoke(false);
        GameEvents.timeCounter.Invoke(true);
    }
    private void OnDestroy() {
        pointer.SetParent(canvas);
    }
    
}