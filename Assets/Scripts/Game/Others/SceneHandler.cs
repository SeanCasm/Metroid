using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
public class SceneHandler : MonoBehaviour
{
    public static SceneHandler current;
    [SerializeField]AssetReference initialRoom;
    [SerializeField] Vector2 spawn;
    private PlayerController pContr{get;set;}
    private GameObject scenario;
     private void Awake() {
        if(current==null){
            current=this;
            DontDestroyOnLoad(gameObject);
        }else Destroy(this);
    }
     public void SetCurrentScenario(GameObject cur){
        scenario=cur;
    }
    public void DestroyScenario()=>Destroy(scenario);
    private void StartingLoad(){
        AsyncOperation operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        StartCoroutine(CheckUnload(operation));
    }
    public void LoadFromNewGame(){
        AnimationHandle.current.StartRetry(StartingLoad);
    }
    public void LoadFromRetryScreen(){
        StartingLoad();
    }
    public void LoadFromSaveGame(Action action)
    {
        if (Slots.retry) StartingLoad();
        else AnimationHandle.current.StartRetry(StartingLoad);
    }
    IEnumerator CheckUnload(AsyncOperation operation)
    {
        pContr = FindObjectOfType<PlayerController>();
        while (!operation.isDone)
        {
            yield return null;
        }
        if(SaveAndLoad.newGame)initialRoom.InstantiateAsync(Vector3.zero, Quaternion.identity, null).Completed+=OnLoadComplete;
        else{
            Addressables.InstantiateAsync("Assets/Prefabs/Scenarios/"+SaveAndLoad.sectorName+".prefab").Completed+=OnLoadComplete;
        }
    }
    void OnLoadComplete(AsyncOperationHandle<GameObject> obj){
        SetCurrentScenario(obj.Result);
        AnimationHandle.current.EnableEnd(EnableAll);
        Slots.retry = false;
        AudioListener.pause=false;
    }
    void EnableAll(){
        pContr.SetTransformCenter(spawn);
        List<Behaviour> behaviours = pContr.GetComponents<Behaviour>().ToList();
        behaviours.Add(pContr.GetComponentInChildren<Gun>());
        pContr.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        foreach (var e in behaviours)
        {
            e.enabled = true;
        }
        Pause.onGame = true;
        Time.timeScale = 1f;
    }
}
