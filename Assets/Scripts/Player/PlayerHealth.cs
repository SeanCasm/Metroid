﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using System;
using UnityEngine.Events;

public class PlayerHealth : Health<int>,IDamageable<int>,IFreezeable
{
    #region Properties
    public static PlayerHealth current;
    [SerializeField] Materials materials;
    [SerializeField] BaseData baseData;
    [SerializeField] float invTime;
    [SerializeField] UnityEvent death;
    private int healthRound=1;
    private float currentTankSize;
    private PlayerController player;
    private AudioSource audioPlayer;
    private GameData data;
    public AudioClip damageClip;
    public int ETanks { get; set; }
    public bool invulnerable{get;set;} public bool freezed { get; set; }
    private bool freezeInvulnerablility;
    public bool CheckCurrentHealth(){
        return (healthRound*health<healthRound*99) ? true : false;
    }
    public bool unFreezing { get; set; }
    public bool isDead{get;private set;}
    #endregion
    #region Unity Methods
    private void Start() {
        if(current==null)current=this;
        anim = GetComponentInChildren<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        audioPlayer = GetComponent<AudioSource>();
        player=GetComponent<PlayerController>();;
    }
    private void Awake() => baseData.SetHealthData(this);
    private void OnEnable() {

        GameEvents.healthTank+=FillHealth;
        GameEvents.refullAll+=HandleRefullAll;
    }
    private void OnDisable()
    {
        GameEvents.refullAll -= HandleRefullAll;
        GameEvents.healthTank-=FillHealth;
    }
    #endregion
    #region Public Methods
    public void FreezeMe(){
        if(!freezeInvulnerablility){
            CancelInvoke("Unfreeze");
            StopAllCoroutines();
            Invoke("Unfreeze", 4f);
            _renderer.material = materials.freeze;
            player.Freeze(true);
            freezed=freezeInvulnerablility = true;
            StartCoroutine(FreezeVisualFeedBack());
        }
    }
    public void Unfreeze()
    {
        Invoke("CanBeFreeze",2.5f);
        _renderer.material = materials.defaultMaterial;
        player.Freeze(false);
        freezed = false;
    }
    public void LoadHealth(GameData data)
    {
        this.data=data;
        healthRound += data.tanks;
        ETanks = data.tanks;
        health = 99;
        currentTankSize = 16f * ETanks;
        GameEvents.playerHealth.Invoke(MyHealth,ETanks);
    }
    /// <summary>
    /// Adds a tank to the total tanks count and refill the player health
    /// </summary>
    private void FillHealth()
    {
        ETanks += 1;
        health = 99;
        healthRound = ETanks + 1;
        GameEvents.playerHealth.Invoke(health,ETanks);
    }
    /// <summary>
    /// Sets damage to the player
    /// </summary>
    /// <param name="amount">amount of damage received</param>
    public void AddDamage(int amount)
    {
        if (!invulnerable)
        {
            invulnerable = true;
            SetDamage(amount);
            StartCoroutine("InvulnerableFeedback");
        }
    }
    public void ConstantDamage(int damageAmount) => SetDamage(damageAmount);
    /// <summary>
    /// Add health points to player health count
    /// </summary>
    /// <param name="amount">amount of health earned</param>
    public void AddHealth(int amount)
    {
        if (health + amount >= 99 && healthRound == ETanks + 1)
        {
            health = 99;
        }
        else
        if (health + amount <= 99)
        {
            health = health + amount;
        }
        else
        if (health + amount > 99 && healthRound < ETanks + 1)
        {
            int healthNext = health + amount - 99;
            health = healthNext;
            healthRound++;
            GameEvents.playerHealth.Invoke(health,ETanks);
            return;
        }
        GameEvents.playerHealth.Invoke(health,ETanks);
    }

    #endregion
    #region Private Methods
    private IEnumerator InvulnerableFeedback(){
        float time=0;
        while(time<invTime){
            _renderer.color=Color.red;
            yield return new WaitForSeconds(.1f);
            _renderer.color = Color.white;
            yield return new WaitForSeconds(.1f);
            time+=.2f;
        }
        invulnerable = false;
        _renderer.color = Color.white;
    }
    private void CanBeFreeze() => freezeInvulnerablility=false;
    public IEnumerator FreezeVisualFeedBack()
    {
        _renderer.color = _renderer.color.Default();
        yield return new WaitForSeconds(2f);
        while (freezed)
        {
            for (float i = 1; i >= 0.5f; i -= .5f)
            {
                Color color=_renderer.color;
                _renderer.color =color.SetColorRGB(i);
                yield return new WaitForSeconds(0.05f);
            }
            for (float i = 0.5f; i <= 1; i += .5f)
            {
                Color color = _renderer.color;
                _renderer.color = color.SetColorRGB(i);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
    public void SetConstantDamage(int amount) => Damage(amount);
    private void SetDamage(int amount)
    {
        Damage(amount);
        audioPlayer.loop=false;
        audioPlayer.ClipAndPlay(damageClip);
    }
    private void Damage(int amount){
        if (health >= amount)
        {
            health -= amount;
            GameEvents.playerHealth.Invoke(health, ETanks);
        }
        else
        if (health < amount)
        {
            healthRound--;
            if (healthRound == 0)
            {
                health = 0;
                death.Invoke();
                OnDeath();
                return;
            }
            if (ETanks > 0) ETanks--;
            int healthPrev = amount - health;
            health = 99 - healthPrev;
            GameEvents.playerHealth.Invoke(health, ETanks);
        }
    }
    private void OnDeath(){
        isDead=true;
        StopAllCoroutines();
        player.ResetState();
        Player.Animation.Death.current.StartAnimation(Retry.Completed,_renderer.transform.eulerAngles.y,player.TransformCenter());
        Retry.Start.Invoke();
        OnMenuHandler.onAnyMenu = AudioListener.pause = true;
        gameObject.SetActive(false);
        Time.timeScale = 0f;
    }
    private void HandleRefullAll()
    {
        this.health=99;this.healthRound=ETanks+1;
        GameEvents.playerHealth.Invoke(health,ETanks);
    }

    public void SetDide(float side)
    {
    }
    #endregion
}