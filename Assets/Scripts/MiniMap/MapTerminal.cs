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
    [SerializeField] GameObject acqPanel,arm,armLeft;
    [SerializeField] Animator light;
    private int id;
    TextMeshProUGUI panelText;
    private bool active = true;
    private Animator animator, pAnim;
    private PlayerController pContr;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        GameDataContainer.instance.mapTerminals.ForEach(item =>
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
            Player.GroundChecker groundChecker = other.GetComponentInParent<Player.GroundChecker>();
            if(groundChecker.isGrounded && !pContr.IsJumping){
                pAnim = pContr.anim;
                if(other.transform.position.x > transform.position.x) arm.SetActive(true);
                else armLeft.SetActive(true);

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
        GameDataContainer.instance.AddMapTerminal(id);
        Destroy(obj, 1f);
        Invoke("TransitionToMinimap", 1f);
    }
    private void ClearReferences()
    {
        panelText = null;
        acqPanel = null;
    }
    private void TransitionToMinimap(){
        float time=GameEvents.instance.StartTransition.Invoke();
        Invoke("EnablePlayer",time/2);
    }
    private void EnablePlayer()
    {
        TurnOff();
        panelText.text = "";
        ClearReferences();
        arm.SetActive(false);
        armLeft.SetActive(false);

        pContr.SetAllInput(true);
        active = false;
        GameEvents.instance.MinimapShortcout.Invoke();
    }
    private void TurnOff(){
        light.gameObject.SetActive(false);
        animator.enabled = false;
        minimapTiles = null;
    }
}