using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Class attached to map revealer GameObjects in scene.
/// </summary>
public class MapTerminal : MonoBehaviour
{
    [SerializeField] MinimapTilesMapUpdater minimapTiles;
    [SerializeField] GameObject acqPanel,arm;
    [SerializeField] Animator light;
    private int id;
    TextMeshProUGUI panelText;
    private bool active = true;
    private Animator animator, pAnim;
    private PlayerController pContr;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        MapSaveSystem.mappers.ForEach(item =>
        {
            if (item == id)
            {
                active = false;
                TurnOff();
                ClearReferences();
                return;
            }
        });
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Gun>() && active)
        {
            pContr = other.GetComponentInParent<PlayerController>();

            if(pContr.leftLook && pContr.isGrounded && !pContr.IsJumping){
                pAnim = other.GetComponentInParent<Animator>();
                arm.SetActive(true);
                SetPlayer();
                PassMapData();
            }
        }
    }
    private void SetPlayer()=>pContr.SetAllInput(false);
    private void PassMapData()
    {
        animator.SetTrigger("Detected");
        light.SetTrigger("Activated");
        minimapTiles.minimapScripts.ForEach(item =>
        {
            item.SetTile();
        });
        //minimapTiles=null;
    }
    /// <summary>
    /// Called in animation event.
    /// </summary>
    public void Completed()
    {
        var obj = Instantiate(acqPanel, References.instance.canvas.position, Quaternion.identity, References.instance.canvas);
        panelText = obj.GetChild(0).GetComponent<TextMeshProUGUI>();
        panelText.text = "Map update completed";
        MapSaveSystem.mappers.Add(id);
        Destroy(obj, 1f);
        Invoke("TransitionToMinimap", 1f);
    }
    private void ClearReferences()
    {
        panelText = null;
        acqPanel = null;
    }
    private void TransitionToMinimap(){
        float time=GameEvents.StartTransition.Invoke();
        Invoke("EnablePlayer",time/2);
    }
    private void EnablePlayer()
    {
        TurnOff();
        panelText.text = "";
        ClearReferences();
        arm.SetActive(false);

        pContr.SetAllInput(true);
        pContr = null;
        pAnim = null;
        active = false;
        GameEvents.MinimapShortcout.Invoke();
    }
    private void TurnOff(){
        light.gameObject.SetActive(false);
        animator.enabled = false;
        minimapTiles = null;
    }
}