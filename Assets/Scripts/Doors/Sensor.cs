using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
using UnityEngine.Events;
using UnityEngine.UI;

public class Sensor : MonoBehaviour
{
    [SerializeField] bool bossDoor;
    [SerializeField] UnityEvent openEvent, closeEvent;
    [SerializeField] Sprite defaultSprite, lockedSprite;
    [SerializeField] WeaponHint weaponHint;
    [SerializeField] string beamTag;
    SpriteRenderer sRen;
    private Animator _animator;
    public AudioClip clip;
    public AudioClip clip2;
    private bool detection;
    private AudioSource audioClip;
    #region Unity Methods
    private void Awake()
    {
        sRen = GetComponent<SpriteRenderer>();
        audioClip = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        if (bossDoor) LockDoor();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!bossDoor)
        {
            if (collision.GetComponent<IPlayerWeapon>() != null &&
                collision.CompareTag(beamTag))
            {
                GameEvents.OnMissileImpact?.Invoke(transform.position);
                _animator.SetTrigger("Detect");
                detection=true;
            }
            else if (collision.GetComponent<IPlayerWeapon>() != null &&
               !collision.CompareTag(beamTag) && weaponHint!=null)
            {
                weaponHint.ShowHintCanvas();
            }
        }
    }
    #endregion
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
