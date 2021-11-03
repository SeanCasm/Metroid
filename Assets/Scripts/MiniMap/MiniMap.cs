using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MiniMap : MonoBehaviour
{
    public bool itemArea, bossArea;
    public Tile mapTile,unexploredTile;
    public Vector3Int cellPos{get;set;}
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<GameComponents>().UpdateTile_MapSaveSystem(this);
        }
    }
    public void SetTile(){
        cellPos = References.instance.map.WorldToCell(transform.position);
        GameEvents.UnexploredMap.Invoke(this);
    } 
}