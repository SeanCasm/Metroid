using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSwapper : MonoBehaviour
{
    [Tooltip("Menu player suit in background image")]
    [SerializeField] Image suit;
    [SerializeField] Suit power,gravity,corrupt; 
    [SerializeField] Materials materials;
    private SpriteRenderer spriteRenderer;
    private bool gravityEquiped;
    private PlayerController playerController;
    public bool Gravity{get=>gravityEquiped;}
    private List<Sprite> suitLeft=new List<Sprite>();
    private List<Sprite> suitRight= new List<Sprite>();
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        print(spriteRenderer);
        playerController=GetComponent<PlayerController>();
        SetPowerSuit();
    }
    public void SetPowerSuit()
    {
        suit.sprite = power.portait;
        suitLeft = new List<Sprite>();
        suitRight=new List<Sprite>();
        foreach (Sprite element in power.suitLeft)
        {
            suitLeft.Add(element);
        }
        foreach (Sprite element in power.suitRight)
        {
            suitRight.Add(element);
        }
        gravityEquiped = false;
    }
    public void SetScrewAttack(bool value)
    {
        if (value) spriteRenderer.material = materials.screwAttack;
        else spriteRenderer.material = materials.defaultMaterial;
    }
    public void SetGravitySuit()
    {
        suit.sprite = gravity.portait;
        suitLeft= new List<Sprite>();
        suitRight=new List<Sprite>();
        foreach(Sprite element in gravity.suitLeft){
            suitLeft.Add(element);
        }
        foreach (Sprite element in gravity.suitRight)
        {
            suitRight.Add(element);
        }
        gravityEquiped = true;
    }
    void LateUpdate()
    {
        if(!PlayerHealth.current.isDead && suitRight.Count>0){
            int index = int.Parse(spriteRenderer.sprite.name);
            spriteRenderer.sprite = playerController.leftLook ? suitLeft[index] : suitRight[index] ;
        }
    }
     
}