using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] UnityEvent mainMenu,pauseMenu;
    public bool fromMenuCalled { get; set; }
    public void Back()
    {
        if (fromMenuCalled)
        {
            mainMenu?.Invoke();
            OnMenuHandler.onSlots=true;
        }
        else
        {
            pauseMenu?.Invoke();
        }
    }
}
