using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
[CreateAssetMenu(fileName = "new BaseDataDebug", menuName = "ScriptableObjects/Player/BaseDataDebug")]
public class BaseDataDebug : BaseData
{
    public override void SetInventoryData(PlayerInventory inventory)
    {

        for (int i = 0; i < 11; i++)
        {
            inventory.AddToItems(i, true);
            inventory.bUtilities.SetButton(i, true);
        }
        inventory.transform.position = spawn;
        inventory.SetSuit();
    }
    public void SetGunData(Gun gun)
    {
        for (int i = 0; i < 4; i++)
        {
            gun.limitedAmmo[i] = new LimitedAmmo(false, i, beams.limitedAmmo[i], missileAmmo, missileAmmo, gun);
        }
    }
}
