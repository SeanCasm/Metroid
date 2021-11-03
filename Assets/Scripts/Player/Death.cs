using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Player.Animation{
    public class Death : MonoBehaviour
    {
        public static Death current;
        private Animator animator;
        private Action Completed,CanvasAnimationCompleted;
        private void Start() {
            if(current==null){
                current=this;
                DontDestroyOnLoad(gameObject);
            }else Destroy(gameObject);

            animator=GetComponent<Animator>();
        }
        public void StartAnimation(Action action,float y,Vector2 position){
            transform.eulerAngles=new Vector3(0,y,0);
            transform.position=position;
            Completed=action;
            animator.SetTrigger("Start");
        }
        public void SyncWithThis(Action action){
            CanvasAnimationCompleted=action;
        }
        private void AnimationFinished(){
            Completed?.Invoke();
            CanvasAnimationCompleted?.Invoke();
        }
    }
}