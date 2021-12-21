using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
public class References : MonoBehaviour
{
    public static References instance;
    public PlayerHealth playerHealth;
    public Gun playerInventory;
    public Tilemap map;
    public Transform canvas;
    private void Awake()
    {
        instance = this;
    }
}
