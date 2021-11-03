using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OverHeatBar : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] GameObject overHeat;
    [SerializeField] float resizeHorBarOverTime, maxBarSize;
    private RectTransform progressHotBar;
    public static Action<float> SetFill;
    private float minHotProgress;
    private bool overHeated;
    IEnumerator coroutine;
     
    #region Unity methods
    void Awake()
    {
        progressHotBar = GetComponent<RectTransform>();
        minHotProgress = progressHotBar.sizeDelta.x;
        coroutine=UnHotBar();
    }
    private void OnEnable() => SetFill+=SetHot;
    private void OnDisable() => SetFill-=SetHot;
    IEnumerator UnHotBar()
    {
        yield return new WaitForSeconds(1f);
        while (progressHotBar.sizeDelta.x > minHotProgress)
        {
            yield return new WaitForSeconds(0.1f);
            progressHotBar.sizeDelta = new Vector2(progressHotBar.sizeDelta.x - resizeHorBarOverTime, progressHotBar.sizeDelta.y);
        }
        progressHotBar.sizeDelta = new Vector2(minHotProgress, progressHotBar.sizeDelta.y);
        inputManager.EnableFireInput();
        inputManager.lockFireInput=overHeated = false;
        overHeat.SetActive(false);
    }
    #endregion
    
    /// <summary>
    /// Adds hot points to hot progress bar.
    /// </summary>
    /// <param name="hotPointsAmount">total hot points to add to the hot progress bar</param>
    public void SetHot(float hotPointsAmount)
    {
        if(!overHeated){
            progressHotBar.sizeDelta = new Vector2(progressHotBar.sizeDelta.x + hotPointsAmount, progressHotBar.sizeDelta.y);
            StopCoroutine(coroutine);
            coroutine = UnHotBar();
            StartCoroutine(coroutine);
            if (progressHotBar.sizeDelta.x >= maxBarSize)
            {
                overHeated =inputManager.lockFireInput= true;
                inputManager.DisableFireInput();
                progressHotBar.sizeDelta = new Vector2(maxBarSize, progressHotBar.sizeDelta.y);
                overHeat.SetActive(true);
                StartCoroutine(coroutine);
            }
        }
    }
     
    public void ResetHeatBar() => progressHotBar.sizeDelta = new Vector2(0, progressHotBar.sizeDelta.y);
}