using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SpeedBooster : MonoBehaviour
{
    [SerializeField] GameObject ghost;
    [SerializeField] float fadeSpeed;
    [SerializeField] float invokeRate;
    [SerializeField] Materials materials;
    public bool isInvoking{get;private set;}
    SpriteRenderer spriteRenderer;
    private void Start() {
        spriteRenderer=GetComponent<SpriteRenderer>();
    }
    private void SetSpeedOnce(bool value)
    {
        spriteRenderer.material.SetFloat("_Fade", 1f);
        if (value) { spriteRenderer.material = materials.speedBooster; }
        else { spriteRenderer.material = materials.defaultMaterial; }
    }
    public void SetSpeedBooster(bool value)
    {
        if (value) { spriteRenderer.material = materials.speedBooster; StartCoroutine("SpeedBoosterFade"); }
        else { spriteRenderer.material = materials.defaultMaterial; StopCoroutine("SpeedBoosterFade"); }
    }
    public void InvokeGhost(){
        if(!isInvoking){
            SetSpeedOnce(true);
            StartCoroutine("Ghost");
            isInvoking=true;
        }
    }
    public void CancelGhost(){
        SetSpeedOnce(false);
        isInvoking=false;
        StopCoroutine("Ghost");
    }
    IEnumerator SpeedBoosterFade()
    {
        while (spriteRenderer.material != materials.defaultMaterial)
        {
            spriteRenderer.material.SetFloat("_Fade", .2f);
            yield return new WaitForSeconds(fadeSpeed);
            spriteRenderer.material.SetFloat("_Fade", 1f);
            yield return new WaitForSeconds(fadeSpeed);
        }
    }
    IEnumerator Ghost(){
        while(true){
            GameObject gg = Instantiate(ghost, transform.position, Quaternion.identity, null);
            gg.transform.eulerAngles = transform.parent.eulerAngles;
            Ghost g = gg.GetComponent<Ghost>();
            yield return new WaitForEndOfFrame();
            g.SetSprite(spriteRenderer.sprite, spriteRenderer.flipX);
            yield return new WaitForSeconds(invokeRate);
        }
    }
}
public enum RunningState
{
    None = 0, Running = 1, MaxSpeed = 2
}
 