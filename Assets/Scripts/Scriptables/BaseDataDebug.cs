using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
[CreateAssetMenu(fileName = "new BaseDataDebug", menuName = "ScriptableObjects/Player/BaseDataDebug")]
public class BaseDataDebug : BaseData
{
    public override void SetInventoryData(PlayerInventory inventory)
    {
        inventory.limitedAmmo[0] = new CountableAmmo(false, 0, beams.limitedAmmo[0], missileAmmo, missileAmmo);
        inventory.limitedAmmo[1] = new CountableAmmo(false, 1, beams.limitedAmmo[1], missileAmmo, missileAmmo);
        inventory.limitedAmmo[2] = new CountableAmmo(false, 2, beams.limitedAmmo[2], missileAmmo, missileAmmo);
        inventory.limitedAmmo[3] = new CountableAmmo(false, 3, beams.limitedAmmo[3], missileAmmo, missileAmmo);
        for (int i = 0; i < 11; i++)
        {
            inventory.AddToItems(i, true);
            inventory.bUtilities.SetButton(i, true);
        }
        inventory.transform.position = spawn;
        inventory.SetSuit();
    }
}
