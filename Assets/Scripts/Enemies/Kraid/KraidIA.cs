using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KraidIA : Boss
{
    [SerializeField] Animator head;
    [SerializeField] AudioClip roarClip;
    [SerializeField] Enemy.Weapon.Pool pool;
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
        pool.SetKraidPool();
        StartCoroutine(RandomHoleShoot());
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
    IEnumerator RandomHoleShoot()
    {
        int index=2;
        while (health.MyHealth > 0)
        {
            yield return new WaitForSeconds(3f);
            if(!clawsInScene[index]){
                BigClaw bg=pool.pool[index].GetComponent<BigClaw>();
                pool.ActivePrevPoolObject();
                bg.OnDissapear(()=>{clawsInScene[index] = false;});
                clawsInScene[index] = true;
            }
            index--;
            if(index <0)index =2;
        }
    }
    private void Roar()
    {
        audioPlayer.Play();
    }
}