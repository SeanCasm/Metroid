using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KraidIA : Boss
{
    [SerializeField] Animator head;
    [SerializeField] AudioClip roarClip;
    private bool[] clawsInScene=new bool[3];
    private BossHealth health;
    private AudioSource audioPlayer;
    private bool onRoar;
    private int hitsCount=0;
    void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        health = GetComponentInChildren<BossHealth>();
        if(roarClip!=null)audioPlayer.clip = roarClip;
    }
    new void Start()
    {
        base.Start();
    }
    private void Update() {
        if(health.Damaged){
            if(!onRoar)OnHit();
            health.Damaged=false;
        }
    }
    void OnHit(){
        hitsCount++;
        if(hitsCount==4){hitsCount = 0; onRoar=true;}
        if (onRoar)
        {
            head.SetTrigger("Damaged");
            CancelInvoke();
            Invoke("Roar", 0.8f);
            Invoke("StopRoar", 2.74f);
        }
    }
    void StopRoar(){
        onRoar=false;
    }
    private void Roar()
    {
        audioPlayer.Play();
    }
}