using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyBoss.Serris{
    public class BodyPart : MonoBehaviour, IDamageable<float>, IInvulnerable,IFreezeUndamageable,IFreezeable
{
        [SerializeField] float bodyHealth;
        [SerializeField] bool invMissiles, invSuperMissiles, invBeams, invBombs, invSuperBombs, invFreeze, invPlasma,invSpazer;
        [SerializeField]Materials materials;
        [SerializeField] HealthConfig healthConfig;

        [System.Serializable]
        public struct HealthConfig
        {
            public Color quarterHealth;
            public Color halfHealth;
            public Color lowHealth;
        }
        [SerializeField]SerrisIA serrisIA;
        private float totalHealth,currentHealth;
        public bool InvMissiles => invMissiles;
        public bool InvSuperMissiles => invSuperMissiles;
        public bool InvBeams => invBeams;
        public bool InvBombs => invBombs;
        public bool InvSuperBombs => invSuperBombs;
        public bool InvFreeze => invFreeze;
        public bool InvPlasma => invPlasma;
        public bool unFreezing { get; set; }
        public bool freezed { get; set; }

        public bool InvSpazer => invSpazer;

        private bool quarterReached, halfReached, lowReached;
        private SpriteRenderer spriteRenderer;
        private Color currentColor;
        private void OnEnable() {
            serrisIA.OnRage+=Unfreeze;
            serrisIA.OnRage+=DefaultColor;
            serrisIA.OutRage+=SetSpriteColor;
        }
        private void OnDisable() {
            serrisIA.OnRage -= Unfreeze;
            serrisIA.OnRage-=DefaultColor;
            serrisIA.OutRage -= SetSpriteColor;
        }
        private void Awake() {
            spriteRenderer=GetComponent<SpriteRenderer>();
             
            totalHealth=bodyHealth*11;
            currentHealth = totalHealth;
            currentColor=spriteRenderer.color;
        }
        private void SetSpriteColor(){
            spriteRenderer.color=currentColor;
        }
        public void AddDamage(float damage)
        {
            if(bodyHealth>0 && !serrisIA.invulnerable){
                bodyHealth -= damage;
                currentHealth-=damage;
                serrisIA.DamageCount++;
                if(currentHealth<totalHealth*3/4 && !quarterReached){
                    currentColor=spriteRenderer.color=healthConfig.quarterHealth;
                    quarterReached=true;
                }else if(currentHealth < totalHealth /2 && !halfReached){
                    currentColor=spriteRenderer.color=healthConfig.halfHealth;
                    halfReached=true;
                }else if(currentHealth < totalHealth *1/4 && !lowReached){
                    currentColor=spriteRenderer.color = healthConfig.lowHealth;
                    lowReached=true;
                }
            }
            if(currentHealth<=0){
                serrisIA.OnDeath();
            }
        }
        public void FreezeMe()
        {
            if(!serrisIA.invulnerable && bodyHealth>0){
                invMissiles=invSuperMissiles=invBeams=invBombs=invSuperBombs=invPlasma=false;
                CancelInvoke("Unfreeze");
                StopAllCoroutines();
                freezed = true;
                Invoke("Unfreeze", 4f);
                spriteRenderer.material = materials.freeze;
                StartCoroutine(FreezeVisualFeedBack());
            }
        }
         
        public void Unfreeze()
        {
            spriteRenderer.material = materials.defaultMaterial;
            freezed = false;
            invMissiles = invSuperMissiles = invBeams = invBombs = invSuperBombs = invPlasma = true;
        }
        private void DefaultColor(){
            StopAllCoroutines();
            spriteRenderer.color=spriteRenderer.color.Default();
        }
        public IEnumerator FreezeVisualFeedBack()
        {
            spriteRenderer.color = spriteRenderer.color.Default();
            yield return new WaitForSeconds(2f);
            while (freezed)
            {
                for (float i = 1; i >= 0.5f; i -= .5f)
                {
                    Color color = spriteRenderer.color;
                    spriteRenderer.color = color.SetColorRGB(i);
                    yield return new WaitForSeconds(0.05f);
                }
                for (float i = 0.5f; i <= 1; i += .5f)
                {
                    Color color = spriteRenderer.color;
                    spriteRenderer.color = color.SetColorRGB(i);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        public void SetDide(float side)
        {
            throw new System.NotImplementedException();
        }
    }

}
 