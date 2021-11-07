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
    [SerializeField] GameObject[] defaultAmmoPrefabs;
    [SerializeField] ButtonUtilities buttonEssentials;
    [SerializeField] PlayerController pContr;
    [SerializeField] AudioMixerGroup mixerToMute;
    [SerializeField] InputManager inputManager;
    private PlayerInventory inventory;
    private float audioAux;
    private SkinSwapper skin;
    private AudioSource audioPlayer;
    private GameObject itemGot, panel;

    #endregion
    #region Public Methods
    private bool CheckRegisterItems(int id, bool isReserve)
    {
        if (isReserve)
        {
            for (int i = 0; i < inventory.reserve.Count; i++)
            {
                if (inventory.reserve[i] == id) return true;
            }
        }
        else if (!isReserve)
        {
            return inventory.playerItems.ContainsKey(id) ? true : false;
        }
        return false;
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
        pContr.enabled=false;
    }
    #endregion
    #region Unity Methods
    private void Start()
    {
        instance = this;
        GameEvents.verifyRegistry += CheckRegisterItems;
        inventory = player.GetComponent<PlayerInventory>();
        audioPlayer = GetComponent<AudioSource>();
        //skin = player.GetComponent<SkinSwapper>();
    }
    private void OnDisable()
    {
        GameEvents.verifyRegistry -= CheckRegisterItems;
    }
    #endregion
    #region Private Methods

    private void ReserveAcquired(ReserveAcquired reserve)
    {
        itemGot = reserve.gameObject;
        var ammo = inventory.limitedAmmo;
        switch (reserve.ItemType)
        {
            case ReserveType.Missile:
                ammo[0].AddCapacity(reserve.Amount);
                break;
            case ReserveType.SuperMissile:
                if (!inventory.CheckLimitedAmmo(1))
                {
                    CountableAmmo newAmmo = new CountableAmmo(false, 1, defaultAmmoPrefabs[0], reserve.Amount, reserve.Amount);
                    ammo[1] = newAmmo;
                    GameUI.enableUI.Invoke(1);
                    GameUI.ammoText.Invoke(1,reserve.Amount);
                }
                else
                {
                    ammo[1].AddCapacity(reserve.Amount);
                    GameUI.ammoText.Invoke(1, ammo[1].actualAmmo);
                }
                break;
            case ReserveType.SuperBomb:
                if (!inventory.CheckLimitedAmmo(2))
                {
                    CountableAmmo newAmmo = new CountableAmmo(false, 2, defaultAmmoPrefabs[1], reserve.Amount, reserve.Amount);
                    ammo[2] = newAmmo;
                    GameUI.enableUI.Invoke(2);
                    GameUI.ammoText.Invoke(2, reserve.Amount);
                }
                else
                {
                    ammo[2].AddCapacity(reserve.Amount);
                    GameUI.ammoText.Invoke(2, ammo[2].actualAmmo);
                }
                break;
            case ReserveType.EnergyTank:
                GameEvents.healthTank.Invoke();
                break;
            case ReserveType.BouncingBomb:
                if (!inventory.CheckLimitedAmmo(3))
                {
                    CountableAmmo newAmmo = new CountableAmmo(false, 3, defaultAmmoPrefabs[2], 10, 10);
                    ammo[3] = newAmmo;
                    GameUI.enableUI.Invoke(3);
                    GameUI.ammoText.Invoke(3, 10);
                }
                else
                {
                    ammo[3].AddCapacity(10);
                    GameUI.ammoText.Invoke(3, ammo[3].actualAmmo);
                }
                break;
        }
        inventory.reserve.Add(reserve.ID);
        audioPlayer.ClipAndPlay(reserveAcquired);
        panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = reserve.nameItem;
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
        pContr.enabled = true;
        inputManager.EnableUIInput();
        mixerToMute.audioMixer.SetFloat("SE volume", audioAux);
        Destroy(panel); Destroy(itemGot.gameObject);
    }
    #endregion
}
