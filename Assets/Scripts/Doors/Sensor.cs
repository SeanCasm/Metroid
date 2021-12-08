using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
using UnityEngine.Events;
using UnityEngine.UI;

public class Sensor : MonoBehaviour
{
    [SerializeField] bool bossDoor,isDefault;
    [SerializeField] UnityEvent openEvent, closeEvent;
    [SerializeField] Sprite defaultSprite, lockedSprite;
    [SerializeField] WeaponHint weaponHint;
    [SerializeField] int weaponTierToUnlock;
    [SerializeField] string idString;
    [SerializeField] Color blueDoorColor;
    public static Dictionary<string,bool> doorsUnlocked=new Dictionary<string, bool>();
    SpriteRenderer sRen;
    private Animator _animator;
    public AudioClip clip;
    public AudioClip clip2;
    private bool isOpenedFirstTime;
    private AudioSource audioClip;
    #region Unity Methods
    private void Awake()
    {
        sRen = GetComponent<SpriteRenderer>();
        audioClip = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        if(isDefault && doorsUnlocked.ContainsKey(idString)){
            var v = doorsUnlocked[idString];
            if(v==true){
                weaponTierToUnlock=0;
                sRen.color=blueDoorColor;
                isOpenedFirstTime=true;
            }
        }
        if (bossDoor) LockDoor();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!bossDoor)
        {
            
            IPlayerWeapon pWeapon = collision?.GetComponent<IPlayerWeapon>();
            if (pWeapon!= null && pWeapon.weaponTier==weaponTierToUnlock)
            {
                _animator.SetTrigger("Detect");
                SetNewDoor();
                isOpenedFirstTime=true;
            }
            else if (pWeapon!=null && pWeapon.weaponTier!=weaponTierToUnlock && weaponHint!=null && !isOpenedFirstTime)
            {
                weaponHint.ShowHintCanvas();
            }
        }
    }
    #endregion
    private void SetNewDoor(){
        if(isDefault){
            if(!doorsUnlocked.ContainsKey(idString)){
                doorsUnlocked.Add(idString,true);
                weaponTierToUnlock = 0;
            }
        }
    }
    public void SetNewDoorColor(){
        sRen.color=blueDoorColor;
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
    }
    public void Close()
    {
        closeEvent.Invoke();
        _animator.SetTrigger("Close");
    }
    public void AudioOpen()
    {
        audioClip.clip = clip;
        audioClip.Play();
    }
    public void AudioClose()
    {
        audioClip.clip = clip2;
        audioClip.Play();
    }

}
