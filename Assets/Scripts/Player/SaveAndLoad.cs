using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerHealth health;
    [SerializeField] PlayerController pContr;
    [SerializeField]MapSaveSystem map;
    private GameData data;
    private float[] position= new float[3];
    public static string sectorName{get;set;}
    public static bool newGame;
    public static int slot{get;set;}
    public void SetPositions(float x, float y, float z)
    {
        position[0] = x;
        position[1] = y;
        position[2] = z;
    }
    public void SavePlayerSlot(int slotIndex) { SaveSystem.SavePlayerSlot(inventory, health,map, position, sectorName, slotIndex); }
    public void LoadPlayerSlot(int slotIndex)
    {
        data = SaveSystem.LoadPlayerSlot(slotIndex);
        if (data == null)
        {
            return;
        }
        else
        {
            slot=slotIndex;
            dataLoader();
        }
    }
    /// <summary>
    /// Loads all the game data
    /// </summary>
    /// <param name="data">reference to GameData script to load the game data</param>
    private void dataLoader()
    {
        sectorName = data.actualSector;
        health.LoadHealth(data);
        map.LoadMap(data);
        Boss.defeateds = new List<int>(data.bossesDefeated);
        TimeCounter.SetTimeAfterLoad(data.time);
        inventory.LoadInventory(data);
        MapSaveSystem.mappers=new List<int>(data.mappers);
        Vector3 position = new Vector3();
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = 0;
        pContr.SetTransformCenter(position);
        SceneHandler.current.LoadStartScene(OnCompleted);
    }
    void OnCompleted(){
        SaveStation.loaded = true;
        List<Behaviour> behaviours = GetComponents<Behaviour>().ToList();
        behaviours.Add(pContr.GetComponentInChildren<Gun>());
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        foreach (var e in behaviours)
        {
            e.enabled = true;
        }
        Pause.onGame = true;
        Slots.retry = false;
        Time.timeScale = 1f;
    }
}
