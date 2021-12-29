using System.Collections.Generic;
using UnityEngine;
using Player;
using Player.Weapon;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{

    #region Properties
    [SerializeField] Beams beams;
    [SerializeField] BaseData baseData;
    [SerializeField] ButtonUtilities buttonEssentials;
    [SerializeField] Gun gun;
    [SerializeField] ItemMenuController imCont;
    [SerializeField] PlayerFXHandler playerFX;
    [SerializeField] SkinSwapper skin;
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] TouchpadHandler touchpadHandler;
    public static System.Action GravitySetted;
    public ButtonUtilities bUtilities{get=>buttonEssentials;set=>buttonEssentials=value;}
    public static System.Action GravityUnsetted;
    private GameData data;
    private PlayerController pCont;
    public Dictionary<int, bool> playerItems { get; set; }= new Dictionary<int, bool>();
    public List<int> reserve { get; set; }=new List<int>();
    #endregion
    #region Unity methods
     
    void Start()
    {
        pCont = GetComponent<PlayerController>();
        baseData.SetInventoryData(this);
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
            case 6:gun.SetBombToPool(beams.bomb);break;
            case 9:SetSpaceJump();break;
            case 5: SetScrewAttack();break;
        }
        buttonEssentials.SetButton(id, selected);
        imCont.SetButtonNavigation();
        gun.DisableAndSetBeam(id);
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
            return selected ? true: false;
        }else return false;
    }
     
    public void LoadInventory(GameData data)
    {
        this.data=data;
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
            if (pCont.OnSpin) playerHealth.invulnerable = false;

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
    
    #endregion
}