using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BuildManager : MonoBehaviour
{
    [Header("Global config")]
    [SerializeField] Button controlSettings;
    [SerializeField] GameObject touchpad;
    [SerializeField] GraphicRaycaster[] raycasters;
    [SerializeField] List<GameObject> backSelectText,videoConfig;
    [SerializeField] GameObject eventSystemAndroid;
    [SerializeField]List<GameObject> mobileButtons;
    [Header("Player UI and inventory config")]
    [SerializeField]List<GameObject> ammoUI;
    [SerializeField]PlayerInventory playerInventory;
    private void Awake()
    {
#if UNITY_ANDROID
        touchpad.SetActive(true);
        videoConfig.ForEach(item=>{
            Destroy(item);
        });
        //controlSettings.interactable=false;
        backSelectText.ForEach(item =>
        {
            item.GetComponent<TMPro.TextMeshProUGUI>().text = "";
        });
#endif
 
#if UNITY_STANDALONE
        Destroy(touchpad);
        mobileButtons.ForEach(item =>
        {
            Destroy(item);
        });

        var raycasters = FindObjectsOfType<GraphicRaycaster>();
        foreach (var item in raycasters)
        {
            item.enabled=false;
        }
        var imageRaycasters = FindObjectsOfType<Image>();
        foreach (var item in imageRaycasters)
        {
            item.raycastTarget=false;
        }
        ammoUI.ForEach(item=>{
            item.GetComponent<EventTrigger>().enabled=true;
        });
#endif
    }
}
