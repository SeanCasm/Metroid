using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DropManager : MonoBehaviour
{ 
    public static DropManager instance;
    public GameObject[] ammoDrops,healthDrops;
    public GameObject reloadAll;
    private void Awake() {
        instance=this;
    }
    /// <summary>
    /// Checks the list of limited ammo for actualAmmo lower than maxAmmo.
    /// </summary>
    /// <param name="playerI"></param>
    /// <returns></returns>
    private List<int> CheckForDrop(Gun playerI)
    {
        int i = 0;
        List<int> ammo = new List<int>();
        while (i < playerI.limitedAmmo.Length)
        {
            if (playerI.limitedAmmo[i] != null)
            {
                var e = playerI.limitedAmmo[i];
                if (e.CheckCapacity())
                {
                    ammo.Add(i);
                }
            }
            i++;
        }
        return ammo;
    }
    private bool CheckForDrop(PlayerHealth playerH)
    {
        if (playerH.CheckCurrentHealth()) return true;
        return false;
    }
    private GameObject SendAmmoDrop(Gun playerInventory)
    {
        var countableAmmo = playerInventory.limitedAmmo;
        List<GameObject> ammoOnInventory = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            if (playerInventory.CheckLimitedAmmo(i))
            {
                var ammo = countableAmmo[i];
                if (ammo.actualAmmo < ammo.maxAmmo) ammoOnInventory.Add(ammoDrops[i]);
            }
        }
        return ammoOnInventory.GetRandom();
    }
    public GameObject TryToDrop()
    {
        PlayerHealth playerHealth = References.instance.playerHealth;
        Gun playerInventory = References.instance.playerInventory;
        bool health = CheckForDrop(playerHealth);
        int i = Random.Range(1, 21);
        var c = CheckForDrop(playerInventory);
        int k=-1;
        print(health);
        if(health && c.Count>0) k =Random.Range(0,2);
        if ((c.Count > 0 && !health) || k==1)
        {
            int a=Random.Range(0,c.Count);
            return ammoDrops[c[a]];
        }else if((health && c.Count==0) || k==0){
            return healthDrops[Random.Range(0,healthDrops.Length)];
        }
        return null;
    }
}