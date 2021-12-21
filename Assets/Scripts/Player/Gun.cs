using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
using System;
using UnityEngine.InputSystem;
using Player.PowerUps;
using UnityEngine.Events;

public class Gun : MonoBehaviour, IFulleable
{
    [SerializeField] InputManager inputManager;
    [SerializeField] Transform firePoint;
    [SerializeField] Beams beams;
    [SerializeField] Pool pool;
    [SerializeField] PlayerController pCont;
    [SerializeField] ButtonUtilities buttonEssentials;
    [SerializeField] SpriteRenderer gunSprite;
    public UnityEvent<int, int> ammoUpdate;
    public UnityEvent<int, bool> viewUpdate;
    public UnityEvent<int> enableItem;
    public bool canShootBeams { get; set; } = true;
    public Action OnStand;
    private int[] beamsID { get; } = { 1, 2, 10 };
    private LimitedAmmo ammo;
    private GameObject chargingLoad;
    private int countableID;
    private PlayerController playerController;
    private PlayerInventory inventory;
    public GameObject beamToShoot { get; set; }
    public FireType fireType { get; set; }
    public Action Fire { get; set; }
    public int pressCount = -1;
    private byte[] gunSelectedID = { 0, 1, 3 };
    public bool canInstantiate { get; set; } = true;
    //0: missiles, 1: super missiles, 2: super bombs, 3: bouncing bomb
    public LimitedAmmo[] limitedAmmo { get; set; } = new LimitedAmmo[4];
    public bool GunMissileAmmoSelected
    {
        get
        {
            foreach (var item in gunSelectedID)
            {
                if (item == pressCount) return true;
            }
            return false;
        }
    }
    void Awake()
    {
        limitedAmmo[0]=new LimitedAmmo(false, 0, beams.limitedAmmo[0], 15, 15, this);
        playerController = GetComponentInParent<PlayerController>();
        chargingLoad = transform.GetChild(0).gameObject;
        inventory = GetComponentInParent<PlayerInventory>();
        SetBeam();
    }
    private void OnEnable()
    {
        inputManager.Fire += FireStarted;
        inputManager.FirePerformed += FirePerformed;
        inputManager.FireCanceled += FireCanceled;
        inputManager.AmmoSelection += SelectingAmmo;
    }
    private void OnDisable()
    {
        inputManager.AmmoSelection -= SelectingAmmo;

        inputManager.Fire -= FireStarted;
        inputManager.FirePerformed -= FirePerformed;
        inputManager.FireCanceled -= FireCanceled;
    }
    public void DisableAndSetBeam(int id)
    {
        foreach (int element in beamsID)
        {
            if (id == element) DisableIncompatibleBeams(id);
        }
        DisableSelection();
        SetBeam();
    }
    public bool CheckLimitedAmmo(int id)
    {
        for (int i = 0; i < limitedAmmo.Length; i++)
        {
            if (limitedAmmo[i] != null)
            {
                if (limitedAmmo[i].iD == id) return true;
            }
        }
        return false;
    }
    public void DisableIncompatibleBeams(int itemID)
    {
        var items = inventory.playerItems;
        if (itemID == 10)
        {
            if (items.ContainsKey(1))
            {
                items[1] = false;
                buttonEssentials.SetButton(1, false);
            }
            if (items.ContainsKey(2))
            {
                items[2] = false;
                buttonEssentials.SetButton(2, false);
            }
        }
        else if (itemID == 2 || itemID == 1)
        {
            if (items.ContainsKey(10))
            {
                items[10] = false;
                buttonEssentials.SetButton(10, false);
            }
        }
    }
    public int AmmoSelection(int itemIndex)
    {
        for (int i = itemIndex; i < limitedAmmo.Length; i++)
        {
            if (itemIndex - 1 >= 0 && limitedAmmo[itemIndex - 1] != null) limitedAmmo[itemIndex - 1].Select(false);//previous ammo selected
            if (limitedAmmo[i] != null && limitedAmmo[i].CheckAmmo())
            {
                limitedAmmo[i].Select(true);
                countableID = i;

                if (i != 2)
                { //super bomb
                    SetLimitedShoot();
                    canShootBeams = false;
                    pool.SetBeamToPool(limitedAmmo[i].ammoPrefab);
                    gunSprite.enabled=true;

                }
                else
                {
                    if (pCont.GroundState != GroundState.Balled)
                    {
                        OnStand = BombSelectionOnStand;
                        OnStand.Invoke();
                    }
                }
                return itemIndex;
            }
            else
            {
                itemIndex++;
            }
        }
        DisableSelection();
        return -1;
    }
    public void UpdateCapacity(int id, int amount)
    {
        limitedAmmo[id].ActualAmmoCount(amount);
    }
    public void SetBombToPool(GameObject bomb)
    {
        pool.SetBombToPool(bomb);
    }
    public void AddNewLimitedAmmo(int i, int ammo)
    {
        limitedAmmo[i] = new LimitedAmmo(false, i, beams.limitedAmmo[i], ammo, this);
        ammoUpdate.Invoke(i, ammo);
        enableItem.Invoke(i);
    }
    public void OnShinesparkCollision() => canInstantiate = true;
    public void OnResetingShinespark()
    {
        if (playerController.status != Status.Damaged)
            canInstantiate = true;
    }
    public void Charge(bool value) => chargingLoad.SetActive(value);

    private void NormalShoot()
    {
        if (canShootBeams)
            pool.ActiveNextPoolObject();
    }
    private void ChargedCancelled()
    {
        if (fireType == FireType.Charged)
        {
            SetChargedShoot();
            Fire();
            SetNormalShoot();
        }
    }
    private void BombSelectionOnStand()
    {
        canShootBeams = true;
        if (countableID == 2)
        {
            SetNormalShoot();
            pool.SetBeamToPool(beamToShoot);
        }
    }
    void Charged() => fireType = FireType.Charged;
    private void SelectingAmmo(InputAction.CallbackContext context)
    {
        pressCount++;
        pressCount = AmmoSelection(pressCount);
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
                playerController.ShootOnWalk = true;
        }
    }
    private void FirePerformed(InputAction.CallbackContext context)
    {
        if (canInstantiate)
        {
            if (inventory.CheckItem(0) && canShootBeams)
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
    private void ChargedShoot()
    {
        pool.ActiveNextChargedPoolObject();
        int id = beamToShoot.GetComponent<Weapon>().ID;
        beamToShoot = beams.GetAmmoPrefab(id * -1);
        //back to normal beam
        beamToShoot = beams.GetAmmoPrefab(id);
    }
    private void LimitedShoot()
    {
        if (countableID == 2) countableID = -999;
        if (CheckLimitedAmmo(countableID))
        {
            var ammoPos = limitedAmmo[countableID];
            pool.ActiveNextPoolObject();
            ammoPos.ActualAmmoCount(-1);
            if (ammoPos.actualAmmo <= 0) SetBeam();
        }
    }
    private LimitedAmmo CreateNewLimitedAmmo(int i, int ammo)
    {
        LimitedAmmo lAmmo = new LimitedAmmo(false, i, beams.limitedAmmo[i], ammo, this);
        ammoUpdate.Invoke(i, ammo);
        enableItem.Invoke(i);
        return lAmmo;
    }
    private void SetBomb()
    {
        ammo = null;
        if (CheckLimitedAmmo(2)) ammo = limitedAmmo[2];
        if (ammo != null)
        {
            if (ammo.CheckAmmo() && ammo.selected)
            {
                GameObject mb = Instantiate(ammo.ammoPrefab, firePoint.position, Quaternion.identity) as GameObject;
                ammo.ActualAmmoCount(-1);
            }
        }
        if ((ammo == null || !ammo.selected) && inventory.CheckItem(6))
        {
            pool.EnableNextBombPoolObject();
        }
    }
#if UNITY_ANDROID
    public void AmmoSelection_Mobile(int index)
    {
        foreach (var i in limitedAmmo)
        {
            if (i!=null && i.iD != index) i.Select(false);
        }

        var lAmmo = limitedAmmo[index];
        lAmmo.Select(!lAmmo.selected);
        gunSprite.enabled=!lAmmo.selected;

        canShootBeams = lAmmo.selected;
        if (lAmmo.selected)
        {
            countableID = index;
            pool.SetBeamToPool(lAmmo.ammoPrefab);
            if (lAmmo.iD != 2)
            {
                SetLimitedShoot();
            }
        }
        else DisableSelection();
    }
#endif
    public void LoadAndCreateLimitedAmmo(int id, GameData data)
    {
        int ammo = 0;
        var ammoMn = data.ammoMunition;
        for (int i = 1; i < ammoMn.Count; i++)
        {
            if (data.ammoMunition.ContainsKey(i)) ammo = ammoMn[i];
            limitedAmmo[i] = CreateNewLimitedAmmo(i, ammo);
        }
        limitedAmmo[0].maxAmmo = limitedAmmo[0].actualAmmo = data.ammoMunition[id];
        ammoUpdate.Invoke(0, limitedAmmo[0].maxAmmo);
    }
    public void SetBeam()
    {
        pressCount = -1;
        SetNormalShoot();
        canShootBeams = true;
        if (inventory.CheckItem(2))//spazer beam
        {
            Ammo.ammoSelected = inventory.CheckItem(1) ? 5 : 7;
        }
        else
        {
            if (!inventory.CheckItem(1))//ice beam
            {
                Ammo.ammoSelected = inventory.CheckItem(10) ? 6 : 3;
            }
            else Ammo.ammoSelected = 4;//ice
        }
        if (inventory.CheckItem(0)) pool.SetChargedBeamToPool(beams.GetAmmoPrefab(-Ammo.ammoSelected));
        var gObj = beams.GetAmmoPrefab(Ammo.ammoSelected);
        pool.SetBeamToPool(gObj);
        beamToShoot = gObj;
    }
    public void DisableSelection()
    {
        foreach (var element in limitedAmmo) if(element!=null) element.Select(false);
        gunSprite.enabled=false;
        SetBeam();
    }
    public void PlayerOnMorphball()
    {
        Charge(false);
        fireType = FireType.Normal;
    }
    public void SetFullCapacity()
    {
        foreach (var item in limitedAmmo) 
        {
            if(item!=null) item.SetFullCapacity();
        }
    }
    public void SetNormalShoot() => Fire = NormalShoot;
    public void SetChargedShoot() => Fire = ChargedShoot;
    public void SetLimitedShoot() => Fire = LimitedShoot;
}
public enum FireType
{
    Normal, Charging, Charged
}