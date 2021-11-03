using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHint : MonoBehaviour
{
    [Header("Hint para abrir la puerta")]
    [SerializeField] Sprite ammo;
    [SerializeField] Image popUp;
    [SerializeField] GameObject canvas;
    private Animator animator;
    private void Start() {
        animator=GetComponent<Animator>();
    }
    public void ShowHintCanvas()
    {
        canvas.SetActive(true);
        popUp.sprite = ammo;
        popUp.SetNativeSize();
    }
    //Se llama en una evento de animacion
    public void HideCanvas(){
        canvas.SetActive(false);
        animator.Rebind();
    }
}
