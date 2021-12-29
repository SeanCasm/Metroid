using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameUI : MonoBehaviour
{
     
    #region Properties
    [SerializeField] PlayerInventory pInventory;
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] RectTransform healthImage;
    [SerializeField] List<GameObject> hudElements;
    [SerializeField] GameObject teleportElement;
    //toggleUI+=UpdateSelection;
      //  enableUI+=SetEnable;
    #endregion
    public void UpdateText(int id,int amount){
        hudElements[id].GetChild(0).GetComponent<TextMeshProUGUI>().text=amount.ToString();
    }
    public void UpdateSelection(int id,bool select){
        hudElements[id].GetChild(1).SetActive(select);
    }
    public void SetEnable(int id){
        hudElements[id].SetActive(true);
    }
    public void UpdateHealth(int amount,int tanks){
        health.text="Energy: "+amount.ToString();
        healthImage.sizeDelta = new Vector2(16f *tanks, 16f);
    }
}