using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Enemy.Weapons;
namespace Enemy.Weapon{
    public class Pool : MonoBehaviour
    {
        [Header("General configuration")]
        [SerializeField] AssetReference bulletPrefab;
        [Tooltip("A transform array where the prefab is shooted, can be one or greater.")]
        [SerializeField] Transform[] shootPoint;
        [SerializeField] int poolSize;
        public Transform[] ShootPoint=>shootPoint;
        public int actual{get;set;}=0;
        public int prev{get;set;}
        private int poolSizeKraid;
        public List<GameObject> pool{get;private set;} = new List<GameObject>();
        private System.Action OnShoot=delegate{};
        private void Awake() {
            if(shootPoint.Length==0){shootPoint=new Transform[1];shootPoint[0]=transform;}
            bulletPrefab.LoadAssetAsync<GameObject>().Completed+=OnLoadDone;
        }
        public void SetKraidPool(){
            poolSizeKraid =poolSize*shootPoint.Length;
            prev=poolSizeKraid-1;
        }
        public void ActiveNextPoolObject(){
            if(actual==poolSize)actual=0;
            OnShoot.Invoke();
            pool[actual].SetActive(true);
            pool[actual].transform.SetParent(null);
            actual++;
        }
        //Solo para Kraid
        public void ActivePrevPoolObject(){
            if (prev <0) prev = poolSizeKraid -1;
            pool[prev].SetActive(true);
            pool[prev].transform.SetParent(null);
            prev--;
        }
        void OnLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj){
            for(int i=0;i<poolSize;i++){
                foreach(Transform item in shootPoint){
                    var gObj=Instantiate(obj.Result, item.position, Quaternion.identity, item);
                    gObj.GetComponent<IPooleable>().parent = item;
                    pool.Add(gObj);
                    gObj.SetActive(false);
                }
            }
        }
        private void OnDestroy() {
            pool.ForEach(item=>{Destroy(item);});
        }
    }
}
 
