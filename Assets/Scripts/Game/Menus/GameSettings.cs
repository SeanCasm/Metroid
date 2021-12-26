using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
public class GameSettings : MonoBehaviour
{
    [SerializeField]AudioMixer audioMixer,musicMixer,onPause;
    [SerializeField] float initialVolumeLevel;
    [SerializeField] Slider soundSlider, musicSlider;
    [SerializeField] AudioClip sampleClip;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Toggle fullScreen,dpadEnabled;
    [SerializeField] RenderPipelineAsset[] qualityLevels;
    [SerializeField] TouchpadHandler touchpadHandler;
    private AudioSource audioS;
    private float volumeLevel, musicLevel;
    private const string soundVolume="SE volume"; 
    private const string musicVolume="MU volume";
    private void Start()
    {
        Application.targetFrameRate=45;
        audioS = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LoadSettings();
        if(PlayerPrefs.HasKey("dpad")){
            int j = PlayerPrefs.GetInt("dpad");
            bool i = j==0 ? true:false;
            touchpadHandler.SwitchCurrentMovementHandler(i);
            dpadEnabled.isOn=i;
        }
        if(PlayerPrefs.HasKey(soundVolume)){
            volumeLevel = PlayerPrefs.GetFloat(soundVolume);
            audioMixer.SetFloat(soundVolume, Mathf.Log10(volumeLevel)*20);
            soundSlider.value = volumeLevel;

        }else audioMixer.SetFloat(soundVolume, Mathf.Log10(initialVolumeLevel) * 20);

        if(PlayerPrefs.HasKey(musicVolume)){
            musicLevel = PlayerPrefs.GetFloat(musicVolume);
            musicMixer.SetFloat(musicVolume,Mathf.Log10(musicLevel)*20);
            musicSlider.value = musicLevel;
            
        }else musicMixer.SetFloat(musicVolume, Mathf.Log10(initialVolumeLevel) * 20);
    }
    #region Sounds volume
   
    public void SetEffectsVolume(float volume)
    {
        audioS.mute=false;
        float newVol=Mathf.Log10(volume)*20;
        audioMixer.SetFloat(soundVolume, volumeLevel=newVol);
        PlayerPrefs.SetFloat(soundVolume,volume);
        if(!audioS.mute)audioS.ClipAndPlay(sampleClip);
        audioS.mute=true;
    }
    public void SetEffectsVolume(bool mute){
        if(mute)audioMixer.SetFloat(soundVolume,-80);
        else audioMixer.SetFloat(soundVolume, volumeLevel);
    }
    #endregion
    #region Music volume
    public void SetMusicVolume(bool decrease){
        if(decrease)musicMixer.SetFloat(musicVolume,-40);
        else musicMixer.SetFloat(musicVolume,musicLevel);
    }
    public void SetMusicVolume(float volume)
    {
        audioS.mute=false;
        float newVol = Mathf.Log10(volume)*20;
        musicMixer.SetFloat(musicVolume, musicLevel = newVol);
        PlayerPrefs.SetFloat(musicVolume,volume);
        if(!audioS.mute)audioS.ClipAndPlay(sampleClip);
        audioS.mute=true;
    } 
    #endregion
    #region Quality
    public void SetQuality(int level){
        QualitySettings.SetQualityLevel(level);
        QualitySettings.renderPipeline=qualityLevels[level];
        PlayerPrefs.SetInt("quality",level);
    }
    #endregion
    private void LoadSettings(){
        if(PlayerPrefs.HasKey("quality")){
            int quality = PlayerPrefs.GetInt("quality");
            QualitySettings.SetQualityLevel(quality);
            QualitySettings.renderPipeline=qualityLevels[quality];
            dropdown.value=quality;
        }
         
        if(PlayerPrefs.HasKey("fullScreen")){
            fullScreen.isOn = PlayerPrefs.GetString("fullScreen").Equals("true");
        }
    }
    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetString("fullScreen",value.ToString().ToLower());
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    
}
