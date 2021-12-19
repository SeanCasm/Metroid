using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SaveStation : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] Transform spawn;
    private SaveAndLoad saveLoad;
    private PlayerController playerController;
    private int gameSlot;
    public string actualSectorLoad;
    public static bool loaded;
    private void OnDisable() {
        loaded=false;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && !loaded && 
        (playerController=col.GetComponentInParent<PlayerController>()).GroundState!=GroundState.Balled)
        {
            GameEvents.instance.save.Invoke(this);
            var gC= col.GetComponent<GameComponents>();
            saveLoad = gC.GetSaveAndLoad;
            inputManager = gC.GetInputManager;
            playerController.ResetState();
            playerController.SetTransformCenter(spawn.position);
            playerController.isGrounded=true;
            loaded=true;
            for(int i=0;i<3;i++){
                if(i==SaveAndLoad.slot)gameSlot=i;
            }
            OnStation();
        }
    }
    public void saveGame(bool optionSelect)
    {
        if (optionSelect)
        {
            saveLoad.SetPositions(spawn.position.x, spawn.position.y, spawn.position.z);
            SaveAndLoad.sectorName = actualSectorLoad;
            inputManager.DisableUIInput();
            saveLoad.SavePlayerSlot(gameSlot);
            PlayerAnimatorUpdate(false, true);
            Invoke("stopSavingAnim",4f);
        }
        else
        {
            Pause.UnpausePlayer();
            unFreezeMoves();
            playerController.SetAnimation(20, false);
            inputManager.EnablePlayerInput();
        }
    }
    #region Private methods
    /// <summary>
    /// Set the player to the save station position, change his animation state and disable all possible movements.
    /// </summary>
    void OnStation()
    {
        playerController.SetAnimation(20, true);
        playerController.SetTransformCenter(spawn.position);
        playerController.SetConstraints(RigidbodyConstraints2D.FreezeAll);
        inputManager.DisablePlayerInput();
    }
    void stopSavingAnim()
    {
        GameEvents.instance.saveMessage.Invoke();
        unFreezeMoves();
        PlayerAnimatorUpdate(false, false);
        inputManager.EnableAll();
    }
    void unFreezeMoves()
    {
        playerController.SetConstraints(RigidbodyConstraints2D.FreezeRotation);
    }
    void PlayerAnimatorUpdate(bool saved, bool isSaving)
    {
        playerController.SetAnimation(20,saved);
        playerController.SetAnimation(17,isSaving);
    }
    #endregion
}