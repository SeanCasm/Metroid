using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Sensor : MonoBehaviour
{
    [SerializeField] bool bossDoor, isDefault;
    [SerializeField] UnityEvent openEvent, closeEvent;
    [SerializeField] Sprite defaultSprite, lockedSprite;
    [SerializeField] WeaponHint weaponHint;
    [SerializeField] int weaponTierToUnlock;
    [SerializeField] int id;
    [SerializeField] Color blueDoorColor;
    [SerializeField] AudioSource audioClip;
    public int ID { get => id; }
    GameDataContainer gameDataContainer;
    SpriteRenderer sRen;
    private Animator _animator;
    public AudioClip clip;
    public AudioClip clip2;
    private bool isOpenedFirstTime;
    #region Unity Methods
    private void Awake()
    {
        sRen = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        gameDataContainer = GameDataContainer.instance;

        if ((isDefault || bossDoor) && gameDataContainer.DoorExist(id))
        {
            weaponTierToUnlock = 0;
            sRen.color = blueDoorColor;
            isOpenedFirstTime = true;
            bossDoor = false;
        }
        if (bossDoor) LockDoor();

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!bossDoor)
        {

            IPlayerWeapon pWeapon = collision?.GetComponent<IPlayerWeapon>();
            if (pWeapon != null && pWeapon.weaponTier == weaponTierToUnlock)
            {
                _animator.SetTrigger("Detect");
                SetNewDoor();
                isOpenedFirstTime = true;
            }
            else if (pWeapon != null && pWeapon.weaponTier != weaponTierToUnlock && weaponHint != null && !isOpenedFirstTime)
            {
                weaponHint.ShowHintCanvas();
            }
        }
    }
    #endregion
    private void SetNewDoor()
    {
        if (isDefault)
        {
            if (!gameDataContainer.DoorExist(id))
            {
                gameDataContainer.AddDoor(id);
                weaponTierToUnlock = 0;
            }
        }
    }
    public void SetNewDoorColor()
    {
        sRen.color = blueDoorColor;
    }
    public void LockDoor()
    {
        _animator.enabled = false; sRen.sprite = lockedSprite;
        bossDoor = true;
    }
    public void EnableDoor()
    {
        sRen.sprite = defaultSprite;
        _animator.enabled = true;
        bossDoor = false;
    }
    public void Open()
    {
        openEvent.Invoke();
        audioClip.PlayOneShot(clip,PlayerPrefs.GetFloat("SE volume"));
    }
    public void Close()
    {
        closeEvent.Invoke();
        _animator.SetTrigger("Close");
        audioClip.PlayOneShot(clip2,PlayerPrefs.GetFloat("SE volume"));
    }
}
