using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Inventory
{
     public GameObject ammoPrefab { get; set; }
    public static int ammoSelected;
    public Ammo(bool selected, int iD, GameObject ammoPrefab) : base(selected, iD)
    {
        this.ammoPrefab = ammoPrefab;
    }
}
