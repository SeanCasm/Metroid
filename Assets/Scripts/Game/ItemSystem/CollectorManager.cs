using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class CollectorManager : MonoBehaviour
{
    #region Properties
    public static CollectorManager instance;
    [SerializeField] UnityEvent Pickup;
    [SerializeField] AudioClip reserveAcquired, itemAcquired;
    [SerializeField] GameObject player, acquiredPanel, canvas;
    [SerializeField] Image suitUI;
    [SerializeField] Suit gravity, corrupt;
    [SerializeField] ButtonUtilities buttonEssentials;
    [SerializeField] AudioMixerGroup mixerToMute;
    [SerializeField] InputManager inputManager;
    [SerializeField] Gun gun;
    [SerializeField] PlayerHealth playerHealth;
    private PlayerInventory inventory;
    private float audioAux;
    private SkinSwapper skin;
    private AudioSource audioPlayer;
    private GameObject itemGot, panel;

    #endregion
    #region Public Methods
    private bool CheckRegisterItems(int id, bool isReserve)
    {
        if (isReserve) return inventory.reserve.Contains(id) ? true : false;
        else return inventory.playerItems.ContainsKey(id) ? true : false;
    }
    private void AddToPlayerInventory(ItemAcquired item)
    {
        buttonEssentials.SetButton(item.ID, true);
        inventory.AddToItems(item.ID, true);
    }
    public void SetPause()
    {
        inputManager.DisableUIInput();
        mixerToMute.audioMixer.GetFloat("SE volume", out audioAux);
        mixerToMute.audioMixer.SetFloat("SE volume", -80);
        Pause.PausePlayer(true);
    }
    #endregion
    #region Unity Methods
    private void Start()
    {
        instance = this;
        GameEvents.instance.CheckExistence += CheckRegisterItems;
        inventory = player.GetComponent<PlayerInventory>();
        audioPlayer = GetComponent<AudioSource>();
        //skin = player.GetComponent<SkinSwapper>();
    }
    private void OnDisable()
    {
        GameEvents.instance.CheckExistence -= CheckRegisterItems;
    }
    #endregion
    #region Private Methods

    private void ReserveAcquired(ReserveAcquired reserve)
    {
        itemGot = reserve.gameObject;
        switch (reserve.ItemType)
        {
            case ReserveType.Missile:
                gun.limitedAmmo[0].AddCapacity(reserve.Amount);
                break;
            case ReserveType.SuperMissile:
                SetReserve(reserve, 1);
                break;
            case ReserveType.SuperBomb:
                SetReserve(reserve, 2);
                break;
            case ReserveType.EnergyTank:
                playerHealth.FillHealth();
                break;
            case ReserveType.BouncingBomb:
                SetReserve(reserve, 3);
                break;
            case ReserveType.Special:
                SetSpecialReserve(reserve);
                break;
        }
        inventory.reserve.Add(reserve.ID);
        audioPlayer.ClipAndPlay(reserveAcquired);
        panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = reserve.nameItem;
    }
    private void SetSpecialReserve(ReserveAcquired reserve){
    }
    private void SetReserve(ReserveAcquired reserve, int index)
    {
        if (!gun.CheckLimitedAmmo(index)) gun.AddNewLimitedAmmo(index, reserve.Amount);
        else gun.limitedAmmo[index].AddCapacity(reserve.Amount);
    }
    private void ItemAcquired(ItemAcquired item)
    {
        if (item.iType == ItemType.Suit)
        {
            suitUI.sprite = gravity.portait;
            skin = player.GetComponent<SkinSwapper>();
            skin.SetGravitySuit();
        }
        itemGot = item.gameObject;
        string itemName = item.name;
        panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName;
        panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Message;
        AddToPlayerInventory(item);
        audioPlayer.ClipAndPlay(itemAcquired);
    }
    public void HandlePickupItem(ItemAcquired itemS)
    {
        Pickup.Invoke();
        ItemAcquired(itemS);
        StartCoroutine(Resume(2f));
    }
    public void HandlePickupReserve(ReserveAcquired reserveItem)
    {
        Pickup.Invoke();
        ReserveAcquired(reserveItem);
        StartCoroutine(Resume(reserveAcquired.length));
    }
    public void ShowAcquiredPanel()
    {
        panel = Instantiate(acquiredPanel, canvas.transform.position, Quaternion.identity, canvas.transform);
    }
    IEnumerator Resume(float audioLenght)
    {
        yield return new WaitForSecondsRealtime(audioLenght);
        Pause.UnpausePlayer();
        inputManager.EnableUIInput();
        mixerToMute.audioMixer.SetFloat("SE volume", audioAux);
        Destroy(panel); Destroy(itemGot.gameObject);
    }
    #endregion
}
