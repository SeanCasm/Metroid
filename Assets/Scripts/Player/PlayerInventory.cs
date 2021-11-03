using System.Collections.Generic;
using UnityEngine;
using Player;
using Player.Weapon;

[System.Serializable]
public class Inventory
{
    public bool selected { get; set; }
    public int iD { get; set; }
    public Inventory(bool selected, int iD)
    {
        this.selected = selected;
        this.iD = iD;
    }
}
[System.Serializable]
public class Ammo : Inventory
{
    public GameObject ammoPrefab { get; set; }
    public static int ammoSelected;
    public Ammo(bool selected, int iD, GameObject ammoPrefab) : base(selected, iD)
    {
        this.ammoPrefab = ammoPrefab;
    }
}
[System.Serializable]
public class CountableAmmo : Ammo
{
    #region Properties
    public int maxAmmo { get; set; }
    public int actualAmmo { get; set; }

    #endregion
    public CountableAmmo(bool selected, int iD, GameObject ammoPrefab,
        int maxAmmo, int actualAmmo) : base(selected, iD, ammoPrefab)
    {
        this.selected = selected;
        this.iD = iD; this.ammoPrefab = ammoPrefab;
        this.maxAmmo = maxAmmo; this.actualAmmo = actualAmmo;
        GameUI.enableUI.Invoke(iD);
        GameUI.ammoText.Invoke(iD, actualAmmo);
        GameEvents.refullAll += HandleRefullAll;

    }
    public CountableAmmo(bool selected, int iD, GameObject ammoPrefab,
        int maxAmmo) : base(selected, iD, ammoPrefab)
    {
        this.selected = false;
        this.maxAmmo = this.actualAmmo = maxAmmo;
        this.ammoPrefab = ammoPrefab;
        GameUI.enableUI.Invoke(iD);
        GameUI.ammoText.Invoke(iD, actualAmmo);
        GameEvents.refullAll += HandleRefullAll;
    }
    #region Public methods
    public void AddCapacity(int amount)
    {
        maxAmmo += amount; actualAmmo += amount;
        GameUI.ammoText.Invoke(iD, maxAmmo);
    }
    public void ActualAmmoCount(int amount)
    {
        actualAmmo += amount;
        if (actualAmmo >= maxAmmo) actualAmmo = maxAmmo;
        else if (actualAmmo <= 0)
        {
            actualAmmo = 0; Select(false);
        }
        GameUI.ammoText.Invoke(iD, actualAmmo);
    }
    public void Select(bool select)
    {
        GameUI.toggleUI.Invoke(iD, select);
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
    private void HandleRefullAll()
    {
        this.actualAmmo = maxAmmo;
        GameUI.ammoText.Invoke(iD, maxAmmo);
    }
    public void Unsubcribe()
    {
        GameEvents.refullAll -= HandleRefullAll;
    }
    #endregion
}
public class PlayerInventory : MonoBehaviour
{
    public static int[] beamsID { get; } = { 1, 2, 10 };

    #region Properties
    [SerializeField] Beams beams;
    [SerializeField] BaseData baseData;
    [SerializeField] ButtonUtilities buttonEssentials;
    [SerializeField]Pool pool;
    [SerializeField] Gun gun;
    [SerializeField] ItemMenuController imCont;
    [SerializeField] PlayerFXHandler playerFX;
    [SerializeField] SkinSwapper skin;
    [SerializeField] TouchpadHandler touchpadHandler;
    public static System.Action GravitySetted;
    public ButtonUtilities bUtilities{get=>buttonEssentials;set=>buttonEssentials=value;}
    public static System.Action GravityUnsetted;
    private GameData data;
    private PlayerController pCont;
    public Dictionary<int, bool> playerItems { get; set; }= new Dictionary<int, bool>();
    public List<int> reserve { get; set; }=new List<int>();
    public bool canShootBeams { get ;set ; }=true;
    //0: missiles, 1: super missiles, 2: super bombs, 3: bouncing bomb
    public CountableAmmo[] limitedAmmo { get; set; }= new CountableAmmo[4];
    #endregion
    #region Unity methods
     
    void Start()
    {
        pCont = GetComponent<PlayerController>();
        baseData.SetInventoryData(this);
        SetBeam();
    }
    void OnDisable()
    {
        foreach(var element in limitedAmmo){if(element!=null)element.Unsubcribe();}
        DisableSelection();
    }
    #endregion
    #region Public methods
    public void AddToItems(int id, bool selected)
    {
        playerItems.Add(id, selected);
        switch(id){
            case 7: ChangeJumpForce();break;
            case 8: SetSpeedBooster(); break;
            case 3: SetSuit(); break;
            case 6:pool.SetBombToPool(beams.bomb);break;
            case 9:SetSpaceJump();break;
            case 5: SetScrewAttack();break;
        }
        foreach (int element in beamsID)
        {
            if (id == element)DisableIncompatibleBeams(id);
        }
        buttonEssentials.SetButton(id, selected);
        imCont.SetButtonNavigation();
        DisableSelection();
        SetBeam();
    }
    //Called from a onClick event
    public void SetSelectedItems(int itemID)
    {
        bool item = playerItems[itemID];
        buttonEssentials.SetButton(itemID, item = !item);
        playerItems[itemID] = item;
        switch(itemID){
            case 5: SetScrewAttack(); break;
            case 9: SetSpaceJump(); break;
            case 4: SwapMorphItem(item); break;
        }
    }
    public void SwapMorphItem(bool active){
        pCont.canMorph = active;
    }
    public void DisableIncompatibleBeams(int itemID)
    {
        var items = playerItems;
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
        }else if(itemID==2 || itemID == 1)
        {
            if (items.ContainsKey(10))
            {
                items[10] = false;
                buttonEssentials.SetButton(10, false);
            }
        }
    }
    /// <summary>
    /// Checks if a item is selected.
    /// 0=charge beam, 1=ice beam, 2=spazer beam, 3=gravity suit, 4=morphball,
    /// 5=screw attack, 6=bomb,7=high jump, 8=speed booster, 9=gravity jump, 10=plasma </summary>
    /// <param name="itemID">item iD to search</param>
    public bool CheckItem(int itemID)
    {
        if (playerItems.ContainsKey(itemID))
        {
            bool selected = playerItems[itemID];
            if (selected)return true;
            else return false;
        }else return false;
    }
    public bool CheckLimitedAmmo(int id){
        for(int i=0;i<limitedAmmo.Length;i++){
            if(limitedAmmo[i]!=null){
                if(limitedAmmo[i].iD == id)return true;
            }
        }
        return false;
    }
    public void LoadInventory(GameData data)
    {
        this.data=data;
        int ammo=0;
        var ammoMn=data.ammoMunition;
        for(int i = 1; i <ammoMn.Count; i++)
        {
            if (data.ammoMunition.ContainsKey(i))ammo =ammoMn[i];
            CountableAmmo lAmmo=new CountableAmmo(false, i, beams.limitedAmmo[i], ammo);
            limitedAmmo[i]=lAmmo;
        }
        limitedAmmo[0].maxAmmo = limitedAmmo[0].actualAmmo=ammoMn[0];
        GameUI.ammoText.Invoke(0, limitedAmmo[0].maxAmmo);
        reserve = new List<int>(data.reserve);
        foreach(var element in data.items){
            
            if(element.id!=4) AddToItems(element.id,element.selected);
            else{
                imCont.SetButtonNavigation();
                buttonEssentials.SetButton(element.id, element.selected);
            }
        }
    }
    public void SetSuit()
    {
        if(CheckItem(3)){
            skin.SetGravitySuit();
            GravitySetted?.Invoke();
            touchpadHandler.SwapSpriteSuits("gravity");
        }
        else {
            skin.SetPowerSuit();
            GravityUnsetted?.Invoke();
            touchpadHandler.SwapSpriteSuits("power");
        }
    }
    public int AmmoSelection(int itemIndex)
    {
        for (int i = itemIndex; i < limitedAmmo.Length; i++)
        {
            if (itemIndex - 1 >= 0 && limitedAmmo[itemIndex - 1] != null) limitedAmmo[itemIndex - 1].Select(false);//previous ammo selected
            if (limitedAmmo[i]!=null && limitedAmmo[i].CheckAmmo())
            {
                limitedAmmo[i].Select(true);
                Gun.countableID = i;

                if (i!=2){ //super bomb
                    gun.SetLimitedShoot();
                    canShootBeams = false;
                    pool.SetBeamToPool(limitedAmmo[i].ammoPrefab);
                }else{
                    if(pCont.GroundState!=GroundState.Balled){
                        gun.OnStand=BombSelectionOnStand;
                        gun.OnStand.Invoke();
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
    #region Mobile Methods
#if UNITY_ANDROID
    public void AmmoSelection_Mobile(int index){
        foreach(var i in limitedAmmo){
            if(i!=null && i.iD!=index) i.Select(false);
        }
        var lAmmo=limitedAmmo[index];
        lAmmo.Select(!lAmmo.selected);
        canShootBeams = lAmmo.selected;
        if(lAmmo.selected){
            Gun.countableID = index;
            pool.SetBeamToPool(lAmmo.ammoPrefab);
            if(lAmmo.iD!=2){
                gun.SetLimitedShoot();
            }
        }else DisableSelection();
    } 
#endif
    #endregion
    public void DisableSelection()
    {
        foreach(var element in limitedAmmo)if(element!=null)element.Select(false);
        SetBeam();
    }
    public void SetSpaceJump(){
        playerFX.StopJumps();
        if (CheckItem(9))
        {
            if (pCont.jumpType == JumpType.Default) pCont.jumpType = JumpType.Space;
            pCont.SetGravityJump();
        }
        else
        {
            if(CheckItem(5))pCont.jumpType=JumpType.Screw;
            else pCont.jumpType = JumpType.Default;
            pCont.SetNormalJump();
        }
    }
    public void SetScrewAttack()
    {
        playerFX.StopJumps();
        if (CheckItem(5))
        {
            if (pCont.jumpType != JumpType.Screw) pCont.jumpType = JumpType.Screw;
        }
        else
        {
            skin.SetScrewAttack(false);
            if (pCont.OnSpin) PlayerHealth.current.invulnerable = false;

            if(CheckItem(9))pCont.jumpType=JumpType.Space;
            else pCont.jumpType = JumpType.Default;
        }
    }
    
    public void ChangeJumpForce()
    {
        if(playerItems.ContainsKey(7)){
            pCont.currentJumpForce=playerItems[7]?baseData.jumpForceUp:baseData.jumpForce;
        }
    }
    public void SetSpeedBooster(){
        if(!CheckItem(8))pCont.SetSpeedToDefault();
        else pCont.SetSpeedToBooster(); 
    }
    public void SetBeam()
    {
        gun.pressCount=-1;
        gun.SetNormalShoot();
        canShootBeams = true;
        if (CheckItem(2))//spazer beam
        {
            Ammo.ammoSelected=CheckItem(1)?5:7;
        }
        else
        {
            if (!CheckItem(1))//ice beam
            {
                Ammo.ammoSelected=CheckItem(10)?6:3;
            }
            else Ammo.ammoSelected = 4;//ice
        }
        if(CheckItem(0))pool.SetChargedBeamToPool(beams.GetAmmoPrefab(-Ammo.ammoSelected));
        var gObj=beams.GetAmmoPrefab(Ammo.ammoSelected);
        pool.SetBeamToPool(gObj);
        gun.beamToShoot = gObj;
    }
    #endregion
    private void BombSelectionOnStand(){
        canShootBeams = true;
        if(Gun.countableID==2){
            gun.SetNormalShoot();
            pool.SetBeamToPool(gun.beamToShoot);
        }
    }
}