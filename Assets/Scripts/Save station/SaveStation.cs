using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SaveStation : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] Transform spawn;
    [SerializeField] GameObject canvasObject;
    private SaveAndLoad saveLoad;
    private PlayerController playerController;
    private Player.AnimatorHandler animatorHandler;
    private int gameSlot;
    public string actualSectorLoad;
    public static bool loaded;
    private void OnDisable()
    {
        loaded = false;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !loaded &&
        (playerController = col.GetComponentInParent<PlayerController>()).GroundState != GroundState.Balled)
        {
            var gC = col.GetComponent<GameComponents>();
            saveLoad = gC.GetSaveAndLoad;
            inputManager = gC.GetInputManager;
            loaded = true;
            for (int i = 0; i < 3; i++)
            {
                if (i == SaveAndLoad.slot) gameSlot = i;
            }
            playerController.OnSaveStation(spawn.position);
            SaveGame();
        }
    }
    private void SaveGame()
    {
        saveLoad.SetPositions(spawn.position.x, spawn.position.y, spawn.position.z);
        SaveAndLoad.sectorName = actualSectorLoad;
        saveLoad.SavePlayerSlot(gameSlot);
        Invoke("StopSave", 3f);
    }
    #region Private methods
    void StopSave()
    {
        playerController.SetConstraints(RigidbodyConstraints2D.FreezeRotation);
        playerController.SetAnimation(15,false);
        inputManager.EnableAll();
        playerController.enabled=true;

        canvasObject.SetActive(true);
        Invoke(nameof(DisableCanvasObject),2f);
    }
    void DisableCanvasObject(){
        canvasObject.SetActive(false);
    }
    #endregion
}