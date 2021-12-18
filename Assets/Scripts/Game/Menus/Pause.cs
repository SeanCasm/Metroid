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
    public static System.Action<bool> OnPauseInput,OnPause;
    public static bool onGame,escPause;
    public GameObject player;
    public GameObject playerMenu;
    private bool enterPause;
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
        escPause=onGame = false;
    }
    #endregion
    private void PauseMenu(InputAction.CallbackContext context)
    {
        if (CheckBeforePause())
        {
            if (Time.timeScale==0)
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
            if (Time.timeScale==0) unpauseEvent.Invoke();
            else { enterPause = true; EnterPause(false); pauseEvent.Invoke(); }
        }
    }
    #region Public Methods
    public static void PausePlayer(bool calledfromItemOrWarp)
    {
        if (!calledfromItemOrWarp) OnPauseInput?.Invoke(true);
        PauseSystem.instance.EnablePlayerAnimator(false);
        PauseSystem.instance.EnablePlayerContoller(false);

        Time.timeScale = 0f;
    }
    public static void UnpausePlayer()
    {
        OnPauseInput?.Invoke(false);
        PauseSystem.instance.EnablePlayerAnimator(true);
        PauseSystem.instance.EnablePlayerContoller(true);

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
        if (player.activeSelf && !enterPause && onGame) return true;
        else return false;
    }
    
    #region UnityEvent
    public void GeneralPause()
    {
        PausePlayer(false);
        OnPause?.Invoke(false);
        gameSettings.SetEffectsVolume(true);
        gameSettings.SetMusicVolume(true);
        Time.timeScale = 0f;
    }
    public void Unpause()
    {
        OnPauseInput?.Invoke(false);
        #if UNITY_ANDROID
        unpauseAndroid.Invoke();
        #endif
        OnPause?.Invoke(true);
        PauseSystem.instance.EnablePlayerContoller(true);
        gameSettings.SetEffectsVolume(false);
        gameSettings.SetMusicVolume(false);
        GameEvents.timeCounter?.Invoke(true);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        playerMenu.SetActive(false);
        escPause=enterPause = false;
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

        GameEvents.timeCounter.Invoke(false);//pauses the time counter.
    }
    private Button GetComponentAtIndex(GameObject someObject, int index)
    {
        return someObject.transform.GetChild(index).GetComponent<Button>();
    }
    #endregion
}