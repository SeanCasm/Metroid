using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.Weapon
{
    public class Spazer : MonoBehaviour
    {
        [SerializeField] GameObject[] spazerChilds;
        private void OnEnable()
        {
            InitSpazerChild();
        }
        private void InitSpazerChild()
        {
            foreach (var item in spazerChilds)
            {
                item.transform.SetParent(transform);
                var spazer = item.GetComponent<Projectil>();
                spazer.parent = transform;
                item.SetActive(true);
            }
        }
    }
}

