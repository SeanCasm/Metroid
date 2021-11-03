using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComponents : MonoBehaviour
{
    [Header("Component references")]
    [SerializeField] SaveAndLoad saveAndLoad;
    [SerializeField] InputManager inputManager;
    [SerializeField] MapSaveSystem mapSaveSystem;
    public MapSaveSystem GetMapSaveSystem=>mapSaveSystem;
    public InputManager GetInputManager=>inputManager;
    public SaveAndLoad GetSaveAndLoad=>saveAndLoad;
    public void UpdateTile_MapSaveSystem(MiniMap miniMap){
        mapSaveSystem.UpdateTile(miniMap);
    }
}
