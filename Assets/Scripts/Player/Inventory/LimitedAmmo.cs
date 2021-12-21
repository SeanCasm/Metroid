using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedAmmo : Ammo
{
     #region Properties
    public int maxAmmo { get; set; }
    public int actualAmmo { get; set; }

    #endregion
    public LimitedAmmo(bool selected, int iD, GameObject ammoPrefab,
        int maxAmmo, int actualAmmo,Gun gun) : base(selected, iD, ammoPrefab)
    {
        this.selected = selected;
        this.iD = iD; this.ammoPrefab = ammoPrefab;
        this.maxAmmo = maxAmmo; this.actualAmmo = actualAmmo;
        this.gun=gun;
        gun.ammoUpdate.Invoke(this.iD,this.actualAmmo);
        gun.enableItem.Invoke(this.iD);

        SetFullCapacity();
    }
    public LimitedAmmo(bool selected, int iD, GameObject ammoPrefab,
        int maxAmmo,Gun playerInventory) : base(selected, iD, ammoPrefab)
    {
        this.selected = false;
        this.maxAmmo = this.actualAmmo = maxAmmo;
        this.ammoPrefab = ammoPrefab;
        this.gun = playerInventory;
        playerInventory.ammoUpdate.Invoke(this.iD,this.actualAmmo);
        playerInventory.enableItem.Invoke(this.iD);
        
        SetFullCapacity();
    }
    #region Public methods
    public void AddCapacity(int amount)
    {
        maxAmmo += amount; actualAmmo += amount;
        gun.ammoUpdate.Invoke(this.iD,this.actualAmmo);
    }
    public void ActualAmmoCount(int amount)
    {
        actualAmmo += amount;
        if (actualAmmo >= maxAmmo) actualAmmo = maxAmmo;
        else if (actualAmmo <= 0)
        {
            actualAmmo = 0; Select(false);
        }
        gun.ammoUpdate.Invoke(this.iD,this.actualAmmo);
    }
    public void Select(bool select)
    {
        gun.viewUpdate.Invoke(this.iD,select);
        this.selected = select;
        ammoSelected = iD;
    }
    public bool CheckAmmo()
    {
        if (actualAmmo > 0) return true;
        else return false;
    }
    public bool CheckCapacity()
    {
        if (actualAmmo < maxAmmo) return true;
        else return false;
    }
    public void SetFullCapacity()
    {
        this.actualAmmo = maxAmmo;
        gun.ammoUpdate.Invoke(this.iD,this.actualAmmo);
    }
    #endregion
}
