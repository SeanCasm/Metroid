using System.Collections;
using System.Collections.Generic;
using UI.Controller;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPointer : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] RectTransform rect;
    private RectTransform previousRectr;
    #region UI
    public void SetPointerPosition(RectTransform buttonRectr)
    {
        rect.SetParent(buttonRectr);
        rect.anchoredPosition=new Vector2(-buttonRectr.sizeDelta.x/2-distance,0);
        rect.localScale = new Vector2(1, 1);
    }
    public void SetPositionItemMenuRight(RectTransform buttonRectr){
        rect.SetParent(buttonRectr);
        rect.anchoredPosition=new Vector2(buttonRectr.sizeDelta.x/2+distance,0);
        rect.localScale=new Vector2(-1,1);
        Mathf.Clamp(rect.localScale.x, -1, 1);
    }
    public void SetPointerPreviousParent(RectTransform buttonRectr){
        previousRectr = buttonRectr;
    }
    public void SetPointerPreviousParentPosition(){
        rect.SetParent(previousRectr);
        rect.anchoredPosition=new Vector2(-previousRectr.sizeDelta.x/2-distance,0);
        rect.localScale=new Vector2(1,1);
    }
    #endregion 
}