using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
using System;
using UnityEngine.InputSystem;
using Player.PowerUps;

public class Gun : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] Transform firePoint;
    [SerializeField] Beams beams;
    [SerializeField] Pool pool;
    [SerializeField] Shinespark shinespark;
    public Action OnStand;
    private LimitedAmmo ammo;
    private GameObject chargingLoad;
    private PlayerController playerController;
    private PlayerInventory inventory;
    public GameObject beamToShoot{get;set;}
    public FireType fireType{get;set;}
    public Action Fire{get;set;}
    public static int countableID;
    public int pressCount=-1;
    public bool canInstantiate{get;set;}=true;
    void Awake()
    {
        playerController=GetComponentInParent<PlayerController>();
        chargingLoad = transform.GetChild(0).gameObject;
        inventory = GetComponentInParent<PlayerInventory>();
    }
    private void OnEnable() {
        inputManager.Fire += FireStarted;
        inputManager.FirePerformed += FirePerformed;
        inputManager.FireCanceled += FireCanceled;
        inputManager.AmmoSelection += SelectingAmmo;
    }
    private void OnDisable() {
        inputManager.AmmoSelection-= SelectingAmmo;

        inputManager.Fire -= FireStarted;
        inputManager.FirePerformed -= FirePerformed;
        inputManager.FireCanceled -= FireCanceled;
    }
    public void OnShinesparkCollision()=>canInstantiate = true;
    public void OnResetingShinespark(){
        if (playerController.status != Status.Damaged)
            canInstantiate = true;
    }
    public void Charge(bool value)=>chargingLoad.SetActive(value);
    private void NormalShoot()
    {
        if(inventory.canShootBeams )
            pool.ActiveNextPoolObject();
    }
    private void ChargedCancelled(){
        if(fireType == FireType.Charged){
            SetChargedShoot(); 
            Fire();
            SetNormalShoot();
        }
    }
    void Charged() => fireType = FireType.Charged;
    private void SelectingAmmo(InputAction.CallbackContext context)
    {
        pressCount++;
        pressCount = inventory.AmmoSelection(pressCount);
        if (pressCount > 3) pressCount = -1;
    }
    private void FireStarted(InputAction.CallbackContext context)
    {
        if (canInstantiate)
        {
            if (!playerController.isGrounded && playerController.GroundState == GroundState.Stand)
                playerController.ShootOnAir();

            if (playerController.GroundState != GroundState.Balled) Fire();
            else SetBomb();

            if (playerController.xInput != 0 && playerController.isGrounded && !playerController.OnSpin && playerController.aimState == AngleAim.None)
                playerController.ShootOnWalk=true;
        }
    }
    private void FirePerformed(InputAction.CallbackContext context)
    {
        if (canInstantiate)
        {
            if (inventory.CheckItem(0) && inventory.canShootBeams)
            {
                fireType = FireType.Charging;
                Charge(true); Invoke(nameof(Charged), 1.8f);
            }
        }
    }
    private void FireCanceled(InputAction.CallbackContext context)
    {
        if (canInstantiate)
        {
            ChargedCancelled();
            fireType = FireType.Normal;
            CancelInvoke(nameof(Charged));
            Charge(false);
        }
    }
    private void ChargedShoot(){
        pool.ActiveNextChargedPoolObject();
        int id = beamToShoot.GetComponent<Weapon>().ID;
        beamToShoot = beams.GetAmmoPrefab(id * -1);
        //back to normal beam
        beamToShoot = beams.GetAmmoPrefab(id);
    }
    private void LimitedShoot(){
        if (countableID == 2) countableID = -999;
        var ammo = inventory.limitedAmmo;
        if (inventory.CheckLimitedAmmo(countableID))
        {
            var ammoPos = ammo[countableID];
            pool.ActiveNextPoolObject();
            ammoPos.ActualAmmoCount(-1);
            if (ammoPos.actualAmmo <= 0) inventory.SetBeam();
        }
    }
    private void SetBomb()
    {
        ammo=null;
        if(inventory.CheckLimitedAmmo(2))ammo=inventory.limitedAmmo[2];
        if(ammo!=null){
            if (ammo.CheckAmmo() && ammo.selected)
            {
                GameObject mb = Instantiate(ammo.ammoPrefab, firePoint.position, Quaternion.identity) as GameObject;
                ammo.ActualAmmoCount(-1);
            }
        }
        if((ammo==null || !ammo.selected) && inventory.CheckItem(6)){
            pool.EnableNextBombPoolObject();
        }
    }
    public void PlayerOnMorphball(){
        Charge(false);
        fireType = FireType.Normal;
    }
    public void SetNormalShoot()=>Fire=NormalShoot;
    public void SetChargedShoot()=>Fire = ChargedShoot;
    public void SetLimitedShoot()=>Fire=LimitedShoot;
}
public enum FireType{
    Normal,Charging,Charged
}