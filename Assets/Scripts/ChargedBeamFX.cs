using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChargedBeamFX : MonoBehaviour
{
    [SerializeField]Animator childAnim;
    public AudioClip chargingClip,chargedClip;
    private AudioSource audioPlayer;
    private Animator anim;
    void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }
    public void PlayChargingClip()
    {
        audioPlayer.clip = chargingClip;
        audioPlayer.loop = false;
        audioPlayer.Play();
    }
    public void StopChargingClip()
    {
        audioPlayer.Stop();
        anim.SetTrigger("Charged");
        //childAnim.SetTrigger("Charged");
    }

    public void PlayChargedClip()
    {
        audioPlayer.clip = chargedClip;
        audioPlayer.loop = true;
        audioPlayer.Play();
    }
}
