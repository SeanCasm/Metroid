using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public PlayerInventory playerInventory;
    public bool selected { get; set; }
    public int iD { get; set; }
    public Inventory(bool selected, int iD)
    {
        this.selected = selected;
        this.iD = iD;
    }
}
