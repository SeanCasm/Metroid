using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
/// <summary>
/// This class represents the retry screen when player is dead.
/// </summary>
public class Retry : MonoBehaviour
{
    #region Properties
    
    [SerializeField]Transform canvas;
    [SerializeField] UnityEvent playerDeath;
    [SerializeField]private GameObject allObjectContainer;
    [SerializeField]AssetReference retryMenuPrefab;
    [SerializeField] FirstSelectedHandler menuFirst;
    private GameObject retryMenu,retryReference;
    private Button retry,mainMenu;
    public static System.Action Start;
    public static System.Action Completed;
    #endregion
    #region Unity Methods
    private void Awake() {
        retryMenuPrefab.LoadAssetAsync<GameObject>().Completed+=OnLoadDone;
    }
    private void OnEnable()
    {
        Completed += EnableRetry;
    }
    private void OnDisable()
    {
        Completed -= EnableRetry;
    }
    #endregion
    #region Private Methods
    private void OnLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj){
        retryReference =obj.Result;
    }
    private void EnableRetry()
    {
        playerDeath.AddListener(()=>InstantiateAndPutOnCanvas());
        playerDeath.Invoke();
    } 
    public void Back2MainMenu(){
        AnimationHandle.current.EnableMainMenu(
            () =>
                {
                    Destroy(allObjectContainer);
                    SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
                }
        );
    }
    private void InstantiateAndPutOnCanvas(){
        retryMenu =Instantiate(retryReference,canvas.position,Quaternion.identity,canvas);
        //Adding events to mainMenu button
        mainMenu=retryMenu.transform.GetChild(1).GetComponent<Button>();
        mainMenu.onClick.AddListener(() =>Back2MainMenu());
        //Adding events to retry button
        retry=retryMenu.transform.GetChild(0).GetComponent<Button>();
        retry.onClick.AddListener(() =>
        {
            AnimationHandle.current.StartRetry(
                ()=>{
                    Slots.retry = true;
                    Destroy(allObjectContainer);
                    SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
                }
            );
        });
      
        //Setting the first select.
        menuFirst.SetGameObjectToEventSystem(retry);
    }
    #endregion    
}