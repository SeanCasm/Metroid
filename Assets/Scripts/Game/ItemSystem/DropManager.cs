using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DropManager : MonoBehaviour
{
    public static DropManager instance;
    public GameObject[] ammoDrops, healthDrops;
    public GameObject reloadAll;
    private void Awake()
    {
        instance = this;
    }
    private LimitedAmmo[] CheckForDrop(Gun playerI)
    {
        return Array.FindAll(playerI.limitedAmmo,item=>item!=null && item.isFullCapacity);
    }
    private bool CheckForDrop(PlayerHealth playerH)
    {
        if (playerH.CheckCurrentHealth()) return true;
        return false;
    }
    public void TryToDrop(Vector2 position)
    {
        PlayerHealth playerHealth = References.instance.playerHealth;
        Gun playerInventory = References.instance.playerInventory;
        bool health = CheckForDrop(playerHealth);
        int i = UnityEngine.Random.Range(1, 21);
        LimitedAmmo[] ammo = CheckForDrop(playerInventory);
        int k = -1;
        if (health && ammo.Length > 0) k = UnityEngine.Random.Range(0, 2);
        if ((ammo.Length > 0 && !health) || k == 1)
        {
            int a = UnityEngine.Random.Range(0, ammo.Length);
            Instantiate(ammoDrops[ammo[a].iD], position, Quaternion.identity);

        }
        else if ((health && ammo.Length == 0) || k == 0)
        {
            Instantiate(healthDrops[UnityEngine.Random.Range(0, healthDrops.Length)], position, Quaternion.identity);
        }
    }
}