using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName="new Room",menuName="ScriptableObjects/Room")]
public class Room : ScriptableObject
{
    public List<GameObject> roomScenarios;
    private Dictionary<string, GameObject> roomSearch=new Dictionary<string, GameObject>();
    public GameObject LoadRoom(string roomName){
        roomSearch = new Dictionary<string, GameObject>();
        foreach (GameObject element in roomScenarios)
        {
            roomSearch.Add(element.name, element);
        }
        if(roomSearch.ContainsKey(roomName)){
             return roomSearch[roomName];
        }
         return null;
     } 
}
