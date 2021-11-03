using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;
public class Pause : MonoBehaviour
{
    #region Properties
    [SerializeField] InputManager inputManager;
    [SerializeField] UnityEvent unpauseEvent, pauseEvent, quickMinimap,unpauseAndroid;
    [SerializeField] GameSettings gameSettings;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] FirstSelectedHandler menuFirst;
    [SerializeField] PlayerController pContr;
    public static System.Action<bool> OnPauseInput,OnPause;
    public static bool gamePaused, onGame,escPause;
    public GameObject player;
    public GameObject playerMenu;
    private bool enterPause;
    /// <summary>
    /// When player starts navigate on the sub-menus from esc or enter menu
    /// </summary>
    public bool onSubMenu { get; set; }
    #endregion
    #region Unity Methods
    private void OnEnable()
    {
        GameEvents.MinimapShortcout += QuickMinimap;
        inputManager.Pause+=PauseMenu;
        inputManager.PlayerPause += Menu;

    }
    private void OnDisable()
    {
        GameEvents.MinimapShortcout -= QuickMinimap;
        inputManager.Pause -= PauseMenu;
        inputManager.PlayerPause -= Menu;
        escPause=gamePaused  = onGame = false;
    }
    #endregion
    private void PauseMenu(InputAction.CallbackContext context)
    {
        if (CheckBeforePause())
        {
            if (gamePaused)
            {
                unpauseEvent.Invoke();
            }
            else { EscPause(); pauseEvent.Invoke(); }
        }
    }
    private void Menu(InputAction.CallbackContext context)
    {
        if (CheckBeforePause())
        {
            if (gamePaused) unpauseEvent.Invoke();
            else { enterPause = true; EnterPause(false); pauseEvent.Invoke(); }
        }
    }
    #region Public Methods
    public static void PausePlayer(bool calledfromItemOrWarp)
    {
        if (!calledfromItemOrWarp) OnPauseInput?.Invoke(true);
        Time.timeScale = 0f;
    }
    public static void UnpausePlayer()
    {
        OnPauseInput?.Invoke(false);
        Time.timeScale = 1f;
    }
#if UNITY_ANDROID
    public void PauseOnMiniMapTouch_Mobile()
    {
        enterPause = true; EnterPause(true); pauseEvent.Invoke();
    }
#endif
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
    #region Private Methods
    private bool CheckBeforePause()
    {
        if (!OnMenuHandler.onAnyMenu && player.activeSelf && !enterPause && !onSubMenu && onGame) return true;
        else return false;
    }
    
    #region UnityEvent
    private void GeneralPause()
    {
        PausePlayer(false);
        OnPause?.Invoke(false);
        gameSettings.SetEffectsVolume(true);
        gameSettings.SetMusicVolume(true);
        //playerC.rb.velocity = Vector2.zero;
        Time.timeScale = 0f;
        gamePaused = true;
    }
    private void Unpause()
    {
        OnPauseInput?.Invoke(false);
        #if UNITY_ANDROID
        unpauseAndroid.Invoke();
        #endif
        pContr.enabled=true;
        OnPause?.Invoke(true);
        gameSettings.SetEffectsVolume(false);
        gameSettings.SetMusicVolume(false);
        GameEvents.timeCounter?.Invoke(true);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        playerMenu.SetActive(false);
        escPause=enterPause = gamePaused = false;
    }
    /// <summary>
    /// Used in Resume button onclick event at playerMenu.
    /// </summary>
    private void UnpauseEvent()
    {
        unpauseEvent.Invoke();
    }
    #endregion
    void QuickMinimap()
    {
        enterPause = true;
        quickMinimap.Invoke(); 
        pauseEvent.Invoke();
    }

    void EnterPause(bool onMobile)
    {
        GeneralPause();
        if (!onMobile) playerMenu.SetActive(true);
    }
    void EscPause()
    {
        GeneralPause();
        pauseMenu.SetActive(true);
        escPause=true;
        Button resume, options;
        resume = GetComponentAtIndex(pauseMenu, 1);
        menuFirst.SetPauseResumeFirst();
        resume.onClick.AddListener(() =>
        {
            unpauseEvent.Invoke();
        });
        //Setting the first select.
        options = GetComponentAtIndex(pauseMenu, 2);

        options.onClick.AddListener(() =>
        {
            onSubMenu = true;
        });

        GameEvents.timeCounter.Invoke(false);//pauses the time counter.
    }
    private Button GetComponentAtIndex(GameObject someObject, int index)
    {
        return someObject.transform.GetChild(index).GetComponent<Button>();
    }
    #endregion
}