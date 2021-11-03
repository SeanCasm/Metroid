using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponBase<T> : MonoBehaviour
{
    [Header("Weapon settings")]
    [SerializeField] protected T damage;
    [SerializeField] protected float livingTime;
    [SerializeField] protected GameObject startSound;

    [SerializeField] protected Vector3 direction;
    protected void OnEnable() {
        if (startSound != null) Instantiate(startSound);
    }
}

