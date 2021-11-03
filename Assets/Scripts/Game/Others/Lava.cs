using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : Fluid
{
    [SerializeField]float damageCall;
    [SerializeField]int damage;
    private AudioSource audioPlayer;
    private void Awake() {
        audioPlayer=GetComponent<AudioSource>();
    }
    new void OnEnable() {
       base.OnEnable(); 
    }
    new void OnDisable() {
        base.OnDisable();
    }
    new void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);
        if(col.CompareTag("Player")){
            playerH=col.GetComponentInParent<PlayerHealth>();
            
            InvokeRepeating("SetDamage",damageCall,damageCall);
        }
    }
    new void OnTriggerExit2D(Collider2D col)
    {
        base.OnTriggerExit2D(col);
        if (col.CompareTag("Player"))
        {
            playerH=null;
            audioPlayer.Stop();
            CancelInvoke("SetDamage");
        }
    }
    void SetDamage(){
        audioPlayer.Play();
        playerH.SetConstantDamage(damage);
    } 
}
