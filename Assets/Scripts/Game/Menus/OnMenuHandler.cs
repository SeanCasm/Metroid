using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMenuHandler : MonoBehaviour
{
    public static bool onItemMenu{get;set;}
    public static bool onSlots { get; set; }
    public static bool onGame { get; set; }
    public static bool onAnyMenu{ get; set; }
    private void OnDisable() {
        onAnyMenu=onItemMenu=onSlots=onGame=false;
    }
     
}
