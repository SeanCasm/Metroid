using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
public class Warp : MonoBehaviour
{
    [SerializeField] AssetReference nextzone;
    [SerializeField] AudioMixerGroup sounds;
    [SerializeField] Transform exit;
    InputManager inputManager;
    private CameraTransition cameraTransition;
    private GameObject currentZone;
    private PlayerController playerController;
    private float vol;

    void Start()
    {
        inputManager=FindObjectOfType<InputManager>();
        currentZone = transform.root.gameObject;
        if(transform.parent.localScale.x>0)cameraTransition=CameraTransition.Left;
        else cameraTransition=CameraTransition.Right;
    }
     
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            sounds.audioMixer.GetFloat("SE volume",out vol);
            sounds.audioMixer.SetFloat("SE volume",-80);
            playerController=other.GetComponentInParent<PlayerController>();
            playerController.enabled=false;
            CurrentCamera.current.MoveTo(cameraTransition);
            StartCoroutine("Resume");
            inputManager.DisableUIInput();
            Pause.PausePlayer(true);
            print(other);

        }
    }
    IEnumerator Resume()
    {
        yield return new WaitForSecondsRealtime(.5f);

        playerController.SetTransformCenter(new Vector3(exit.position.x,playerController.transform.position.y,0));
        transform.SetParent(null);
        currentZone.SetActive(false);
        nextzone.InstantiateAsync().Completed+=(g)=>{
            SceneHandler.current.SetCurrentScenario(g.Result);
        };
        playerController.enabled = true;
        yield return new WaitForSecondsRealtime(1f);
        sounds.audioMixer.SetFloat("SE volume",vol);
        inputManager.EnableUIInput();
        Pause.UnpausePlayer();
        Destroy(currentZone);
        Destroy(gameObject);
    }
}
