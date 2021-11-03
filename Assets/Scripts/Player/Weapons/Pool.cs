using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Weapon{
    public class Pool : MonoBehaviour
    {
        [Header("General configuration")]
        [SerializeField] Beams beams;
        [Tooltip("A transform array where the prefab is shooted.")]
        [SerializeField] Transform shootPoint;
        [SerializeField] int poolSize,chargedPoolSize,bombPoolSize;
        public int actual { get; set; } = 0;
        public int actualCP{get;set;}=0;
        public int actualBP{get;set;}=0;
        public static System.Action OnPoolChanged;

        public List<GameObject> pool { get; private set; } = new List<GameObject>();
        public List<GameObject> bombPool{get;set;}=new List<GameObject>();
        public List<GameObject> chargedPool{get;set;}=new List<GameObject>();
        /// <summary>
        /// Sets the actual beam selected from player inventory to the pool. The pool is cleared at beginning.
        /// </summary>
        /// <param name="beamPrefab"></param>
        public void SetBeamToPool(GameObject beamPrefab){
            if(!beamPrefab.GetComponent<IPooleable>().pooleable)return;
            OnPoolChanged?.Invoke();
            pool.ForEach(item=>{
                if(!item.activeSelf)Destroy(item);
            });
            pool.Clear();
            for (int i = 0; i < poolSize; i++)
            {
                var gObj = Instantiate(beamPrefab, shootPoint.position, Quaternion.identity, shootPoint);
                gObj.GetComponent<IPooleable>().parent = shootPoint;
                var component = gObj.GetComponent<Projectil>();
                if(component!=null && component.IsSpazer){
                    gObj.GetChild(0).GetComponent<IPooleable>().parent=gObj.transform;
                    gObj.GetChild(1).GetComponent<IPooleable>().parent = gObj.transform;
                }
                pool.Add(gObj);
                gObj.SetActive(false);
            }
        }
        public void SetBombToPool(GameObject prefab){
            for(int i=0;i<bombPoolSize;i++){
                var gObj = Instantiate(prefab, shootPoint.position, Quaternion.identity, shootPoint);
                gObj.GetComponent<IPooleable>().parent = shootPoint;
                bombPool.Add(gObj);
                gObj.SetActive(false);
            }
        }
        public void SetChargedBeamToPool(GameObject beamPrefab){
            if (!beamPrefab.GetComponent<IPooleable>().pooleable) return;
            OnPoolChanged?.Invoke();
            chargedPool.ForEach(item =>
            {
                if (!item.activeSelf) Destroy(item);
            });
            chargedPool.Clear();
            for (int i = 0; i < chargedPoolSize; i++)
            {
                var gObj = Instantiate(beamPrefab, shootPoint.position, Quaternion.identity, shootPoint);
                gObj.GetComponent<IPooleable>().parent = shootPoint;
                var component = gObj.GetComponent<Projectil>();

                if (component != null && component.IsSpazer)
                {
                    gObj.GetChild(0).GetComponent<Projectil>().parent = gObj.transform;
                    gObj.GetChild(1).GetComponent<Projectil>().parent = gObj.transform;
                }
                chargedPool.Add(gObj);
                gObj.SetActive(false);
            }
        }
        public void EnableNextBombPoolObject(){
            if (actualBP == bombPoolSize) actualBP = 0;
            bombPool[actualBP].SetActive(true);
            bombPool[actualBP].transform.SetParent(null);
            actualBP++;
        }
        public void ActiveNextChargedPoolObject(){
            if (actualCP == chargedPoolSize) actualCP = 0;
            chargedPool[actualCP].SetActive(true);
            chargedPool[actualCP].transform.SetParent(null);
            actualCP++;
        }
        public void ActiveNextPoolObject()
        {
            if (actual == poolSize) actual = 0;
            pool[actual].SetActive(true);
            pool[actual].transform.SetParent(null);
            actual++;
        }
    }
}