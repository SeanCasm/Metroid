using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameData
{
    public int tanks;
    public float[] position = new float[2];
    public string actualSector;
    public List<int> reserve = new List<int>();
    public Dictionary<int, int> ammoMunition = new Dictionary<int, int>();//id and max munition
    public List<(int id, bool selected)> items = new List<(int id, bool selected)>();
    public List<int> bossesDefeated = new List<int>();
    public Dictionary<float, float> tilesRegistered = new Dictionary<float, float>();
    public List<int> mappers = new List<int>();
    public List<(string name, int xpos, int ypos)> tileInfo { get; set; } = new List<(string name, int xpos, int ypos)>();
    public List<(string name, int xpos, int ypos)> tileInfoUnexplroed { get; set; } = new List<(string name, int xpos, int ypos)>();
    public List<int> doorsUnlocked= new List<int>();
    public int[] time = new int[4];
    public bool[] miniMapItem = new bool[40];
    public GameData(PlayerInventory inventory, PlayerHealth energy, MapSaveSystem map, float[] pos, string sectorName,Gun gun)
    {
        GameDataContainer gameDataContainer = GameDataContainer.instance;
            actualSector = sectorName;
            var limitedAmmo = gun.limitedAmmo;
            for (int i = 0; i < limitedAmmo.Length; i++)
            {
                if (limitedAmmo[i]!=null)
                {
                    if(limitedAmmo[i].maxAmmo > 0)ammoMunition.Add(limitedAmmo[i].iD, limitedAmmo[i].maxAmmo);
                }
            }
            tileInfo =  new List<(string name, int xpos, int ypos)>(map.tileInfo);
            tileInfoUnexplroed= new List<(string name, int xpos, int ypos)>(map.tileInfoUnexplored);
            time[0] = TimeCounter.hours;
            time[1] = TimeCounter.minutes;
            time[2] = TimeCounter.seconds;
            time[3] = TimeCounter.miliseconds;
            reserve = new List<int>(inventory.reserve);
            SelectItemsData(inventory);
            bossesDefeated = new List<int>(gameDataContainer.bossesDefeated);
            tanks = energy.ETanks;
            position[0] = pos[0];
            position[1] = pos[1];
            mappers = new List<int>(gameDataContainer.mapTerminals);
            miniMapItem = map.miniMapItem;
            doorsUnlocked = new List<int>(gameDataContainer.doorsUnlocked);
    }
    void SelectItemsData(PlayerInventory inventory)
    {
        var items = inventory.playerItems;
        foreach (var element in items)
        {
            this.items.Add((element.Key, element.Value));
        }
    }
}