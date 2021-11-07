using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class TouchpadHandler : MonoBehaviour
{
    [SerializeField] GameObject[] otherButtons;
    [SerializeField] GameObject directionalPad;
    [SerializeField] GameObject dpad,virtualGamepad;
    private GraphicRaycaster graphicRaycaster;
    [Header("Personalizacion")]
    [SerializeField] Image beamButton;
    [SerializeField] Image[] suitButtons;
    [SerializeField] Sprite[] powerSuitSpriteButtons,gravitySuitSpriteButtons;
 
    private void Start() {
        graphicRaycaster=GetComponent<GraphicRaycaster>();
    } 
    //Called in a UnityEvent from MapController
    public void OnMap(bool enable){
        directionalPad.GetComponent<Canvas>().overrideSorting=enable;
        foreach(var e in otherButtons){
            e.SetActive(!enable);
        }
        //graphicRaycaster.enabled=!enable;
    }
    //Unity button onClick event
    public void HideVirtualGamepad(Image image){
        virtualGamepad.SetActive(!virtualGamepad.activeSelf);
        image.color = !virtualGamepad.activeSelf ?  new Color(1,1,1,.5f) : new Color(1,1,1,1f);
    }
    public void SwapSpriteSuits(string suit){
        suit = suit.ToLower();
        switch(suit){
            case "gravity":
                for(int i = 0; i < suitButtons.Length ; i++){
                    suitButtons[i].sprite = gravitySuitSpriteButtons[i];
                }
            break;
            case "power":
                for(int i = 0; i < suitButtons.Length ; i++){
                    suitButtons[i].sprite = powerSuitSpriteButtons[i];
                }
            break;
        }
    }
    public void SwitchCurrentMovementHandler(bool cur){
        if(cur){
            dpad.SetActive(true);
            directionalPad.SetActive(false);
        }else{
            dpad.SetActive(false);
            directionalPad.SetActive(true);
        }
        int i = cur ? 0:1;
        PlayerPrefs.SetInt("dpad",i);
    }
}
