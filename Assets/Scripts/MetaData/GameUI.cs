using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameUI : MonoBehaviour
{
     
    #region Properties
    [SerializeField] PlayerInventory pInventory;
    [SerializeField]PlayerHealth pHealth;
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] RectTransform healthImage;
    [SerializeField] List<GameObject> hudElements;
    public static Action<int, int> ammoText;
    public static Action<int, bool> toggleUI;
    public static Action<int> enableUI;
    #endregion
    private void OnEnable() {
        GameEvents.playerHealth += UpdateHealth;
        ammoText+=UpdateText;
        toggleUI+=UpdateSelection;
        enableUI+=SetEnable;
    }
    private void OnDisable() {
        GameEvents.playerHealth -= UpdateHealth;
        ammoText -= UpdateText;
        toggleUI -= UpdateSelection;
        enableUI -= SetEnable;
    }
    private void Start()
    {
        GameEvents.playerHealth.Invoke(pHealth.MyHealth,pHealth.ETanks);
    }
    private void UpdateText(int id,int amount){
        hudElements[id].GetChild(0).GetComponent<TextMeshProUGUI>().text=amount.ToString();
    }
    private void UpdateSelection(int id,bool select){
        hudElements[id].GetChild(1).SetActive(select);
    }
    private void SetEnable(int id){
        hudElements[id].SetActive(true);
    }
    private void UpdateHealth(int amount,int tanks){
        health.text="Energy: "+amount.ToString();
        healthImage.sizeDelta = new Vector2(16f *tanks, 16f);
    }
}