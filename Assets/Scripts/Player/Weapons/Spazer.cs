using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Weapon;
public class Spazer : MonoBehaviour
{
    [Tooltip("Projectil component of parent gameobject.")]
    [SerializeField] Projectil projectil;
    [SerializeField]Projectil localProjectil;
    [Tooltip("A list of behaviours to disable at collision.")]
    [SerializeField]List<Behaviour> behaviours;
    [SerializeField] float yAxisRelativeToParent;
    [SerializeField]SpriteRenderer spriteRenderer;
    [SerializeField]Rigidbody2D rb2d;
    bool canBack,isActive;
    private void OnEnable() {
        canBack=false;
        isActive=true;
        transform.SetParent(null);
        projectil.OnParentDisabled+=CanBackToParent;
        localProjectil.OnChildCollided+=OnCollision;
    }
    private void OnCollision(){
        isActive = false;
        if(canBack)
        {
            PutBackToParent();
        }else{
            behaviours.ForEach(item =>
            {
                item.enabled = false;
            });
            spriteRenderer.enabled=false;
            rb2d.bodyType=RigidbodyType2D.Static;
        }
    }
    private void PutBackToParent(){
        transform.SetParent(projectil.gameObject.transform);
        transform.position = transform.parent.position;
        transform.localPosition=new Vector3(transform.localPosition.x,transform.localPosition.y+yAxisRelativeToParent,0);
        behaviours.ForEach(item =>
        {
            item.enabled = true;
        });
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        spriteRenderer.enabled=true;
        localProjectil.OnChildCollided -= OnCollision;
        projectil.OnParentDisabled -= CanBackToParent;
    }
    private void CanBackToParent(){
        canBack=true;
        if(!isActive)PutBackToParent();
    }
}