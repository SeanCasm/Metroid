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

    public static List<int> mappers = new List<int>();
    #endregion
    #region Unity Methods
    void Awake()
    {
        mappers = new List<int>();
        miniMapItem = new bool[40];
    }
    void OnEnable()
    {
        GameEvents.UnexploredMap += SetUnexploredTile;
    }
    private void OnDisable()
    {
        GameEvents.UnexploredMap -= SetUnexploredTile;
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
            tileInfo.Add((tileMap.name, cellPos.x, cellPos.y));
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
        Dictionary<string, Tile> tiles1 = LoadFromResources("0");
        Dictionary<string, Tile> tiles2 = LoadFromResources("1");
        Dictionary<string, Tile> tiles3 = LoadFromResources("2");
        Dictionary<string, Tile> tiles4 = LoadFromResources("3");
        Dictionary<string, Tile> tiles5 = LoadFromResources("4");
        Dictionary<string, Tile> tiles6 = LoadFromResources("5");
        tileInfo.ForEach(item =>
        {
            Vector3Int newPos = new Vector3Int(item.xpos, item.ypos, 0);
            switch (item.name[0])
            {
                case '0':
                    tileMap.SetTile(newPos, tiles1[item.name]);
                    break;
                case '1':
                    tileMap.SetTile(newPos, tiles2[item.name]);
                    break;
                case '4':
                    tileMap.SetTile(newPos, tiles5[item.name]);
                    break;
            }
        });
        tileInfoUnexplored.ForEach(item =>
        {
            Vector3Int newPos = new Vector3Int(item.xpos, item.ypos, 0);
            switch (item.name[0])
            {
                case '2':
                    tileMap.SetTile(newPos, tiles3[item.name]);
                    break;
                case '3':
                    tileMap.SetTile(newPos, tiles4[item.name]);
                    break;
                case '5':
                    tileMap.SetTile(newPos, tiles6[item.name]);
                    break;
            }
        });
    }
    private Dictionary<string, Tile> LoadFromResources(string route)
    {
        return Resources.LoadAll(route, typeof(Tile)).Cast<Tile>().ToDictionary(item => item.name, item => item);
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