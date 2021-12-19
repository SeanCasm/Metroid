using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public class MapSaveSystem : MonoBehaviour
{
    #region Properties
    [SerializeField] Tilemap tileMap;
    [SerializeField] Transform playerIcon;
    public List<(string name, int xpos, int ypos)> tileInfo { get; set; } = new List<(string name, int xpos, int ypos)>();
    public List<(string name, int xpos, int ypos)> tileInfoUnexplored { get; set; } = new List<(string name, int xpos, int ypos)>();
    public bool[] miniMapItem { get; set; }
    private Vector3Int cellPos;

    #endregion
    #region Unity Methods
    void Awake()
    {
        miniMapItem = new bool[40];
    }
    void OnEnable()
    {
        GameEvents.instance.UnexploredMap += SetUnexploredTile;
    }
    private void OnDisable()
    {
        GameEvents.instance.UnexploredMap -= SetUnexploredTile;
    }
    #endregion
    public void UpdateTile(MiniMap miniMap){
        cellPos = tileMap.WorldToCell(miniMap.transform.position);
        var iconLocation = tileMap.CellToWorld(cellPos);
        playerIcon.position = new Vector3(iconLocation.x+1.75f, iconLocation.y+1.75f, 0f);
        TileRegister(miniMap);
    }
    #region Private Methods
    private void TileRegister(MiniMap miniMap)
    {
        if (tileMap.GetTile(cellPos) != null)
        {
            var tile = tileMap.GetTile(cellPos);
            if (tile.name[0] == '3' || tile.name[0] == '2')
                tileInfoUnexplored.Remove((tile.name, cellPos.x, cellPos.y));
        }
        var tile2 = tileMap.GetTile(cellPos);
        if (tile2 == null || tile2.name[0] == '3' || tile2.name[0] == '2')
        {
            tileInfo.Add((miniMap.mapTile.name, cellPos.x, cellPos.y));
        }
        tileMap.SetTile(cellPos, miniMap.mapTile);
    }
    void SetUnexploredTile(MiniMap miniMap)
    {
        if (tileMap.GetTile(tileMap.WorldToCell(miniMap.transform.position)) == null)
        {
            tileMap.SetTile(miniMap.cellPos, miniMap.unexploredTile);
            tileInfoUnexplored.Add((miniMap.unexploredTile.name, miniMap.cellPos.x, miniMap.cellPos.y));
        }
    }
    private void SetTilesToTilemap()
    {
        tileInfo.ForEach(item =>
        {
            Vector3Int newPos = new Vector3Int(item.xpos, item.ypos, 0);
            tileMap.SetTile(newPos, LoadFromResources(item.name[0].ToString()+'/'+item.name));
        });
        tileInfoUnexplored.ForEach(item =>
        {
            print(item);
            Vector3Int newPos = new Vector3Int(item.xpos, item.ypos, 0);
            tileMap.SetTile(newPos, LoadFromResources(item.name[0].ToString()+'/'+item.name));
        });
    }
    private Tile LoadFromResources(string route)
    {
        return Resources.Load<Tile>(route);
    }
    #endregion
    /// <summary>
    /// Load data to the MapSaveSystem.
    /// </summary>
    /// <param name="data">the game data</param>
    public void LoadMap(GameData data)
    {
        tileMap.ClearAllTiles();
        miniMapItem = data.miniMapItem;
        tileInfo = new List<(string name, int xpos, int ypos)>(data.tileInfo);
        tileInfoUnexplored = new List<(string name, int xpos, int ypos)>(data.tileInfoUnexplroed);
        SetTilesToTilemap();
    }
}