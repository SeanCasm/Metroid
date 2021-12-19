using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [SerializeField] GameObject bossToSpawn;
    [SerializeField]List<Sensor> doors;
    [SerializeField]PathCreation.PathCreator ridleyPath;
    void Awake()
    {
        int iD = bossToSpawn.GetComponent<Boss>().ID;
        if (!GameDataContainer.instance.CheckBossExistence(iD))
        {
            
            GameObject boss = Instantiate(bossToSpawn, transform.position, Quaternion.identity);
            Boss bossComponent = boss.GetComponent<Boss>();

            foreach(Sensor s in doors){
                bossComponent.roomDoors.Add(s);
            }
            if(iD==1 || iD==2){//ridley
                PathCreation.Examples.PathFollower ridleyPathFollower=boss.GetComponent<PathCreation.Examples.PathFollower>();
                ridleyPathFollower.pathCreator=ridleyPath;
            }
        }
    }
 
}
