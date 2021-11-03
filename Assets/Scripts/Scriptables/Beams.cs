using System.Collections;
using System.Collections.Generic;
using Player.Weapon;
using UnityEngine;
[CreateAssetMenu(fileName ="new Beams",menuName ="ScriptableObjects/Beams")]
public class Beams : ScriptableObject
{
    public GameObject  bomb,chargingLoad;
    public GameObject[] limitedAmmo;
    public GameObject[] beams;
    /// <summary>
    /// Gives the correct ammo prefab by iD.
    /// </summary>
    /// <param name="iD">beam ammo iD</param>
    /// <returns></returns>
    public GameObject GetAmmoPrefab(int iD){
        foreach(GameObject element in beams){
            if(element.GetComponent<Weapon>().ID==iD){
                return element;
            }
        }
        return null;
    }
}
