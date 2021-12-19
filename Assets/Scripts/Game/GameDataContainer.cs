using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataContainer : MonoBehaviour
{
    public static GameDataContainer instance;
    public List<int> bossesDefeated{get;private set;} = new List<int>();
    public List<int> mapTerminals{get;private set;} = new List<int>();
    
    private void Awake() {
        instance=this;
    }
    public void AddBoss(int id){
        bossesDefeated.Add(id);
    }
    public bool CheckBossExistence(int id){
        return bossesDefeated.Contains(id);
    }
    public void InitBossList(List<int> newList){
        bossesDefeated = newList;
    }

    public void InitMapTerminalsList(List<int> newList){
        mapTerminals = newList;
    }
    public void AddMapTerminal(int id){
        mapTerminals.Add(id);
    }
}
