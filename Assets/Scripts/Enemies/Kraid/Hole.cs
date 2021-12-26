using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EnemyBoss.Kraid
{
    public class Hole : MonoBehaviour
    {
        [SerializeField] float timeBeforeShoot, repeatRate;
        [SerializeField] GameObject bigClaw;
        private void OnEnable()
        {
            StartCoroutine("Shoot",timeBeforeShoot);
            bigClaw.GetComponent<BigClaw>().OnDisable += NewShoot;
        }

        private void NewShoot()
        {
            bigClaw.transform.position = transform.position;
            bigClaw.SetActive(false);
            StartCoroutine("Shoot",repeatRate);
        }
        IEnumerator Shoot(float time)
        {
            yield return new WaitForSeconds(time);
            bigClaw.SetActive(true);
        }
    }

}
