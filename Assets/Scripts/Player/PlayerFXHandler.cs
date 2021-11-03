using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerFXHandler : MonoBehaviour
{
    [SerializeField] AudioSource morphjumpAS, nJumpAS, balledAS,spaceAS,spaceASLoop, screwAS, screwLoopAS, stepsAS, rLoopAS, hyperJumpAS, spinJumpAS;
    [SerializeField] AudioClip[] playerSteps;
    /// <summary>
    /// Plays the step sounds in player animation events.
    /// </summary>
    public void Steps()
    {
        StopAllCoroutines();
        int i=Random.Range(0,playerSteps.Length);
        stepsAS.clip=playerSteps[i];
        stepsAS.Play();
    }
    /// <summary>
    /// Plays the normal jump sound in player animation events.
    /// </summary>
    public void NormalJump()
    {
        StopAllCoroutines();
        StartCoroutine(DeactiveGameObject(nJumpAS));
    }
    public void RollJump(JumpType jumpType)
    {
        switch(jumpType){
            case JumpType.Space:
                if (!spaceAS.isPlaying && !spaceASLoop.isPlaying)
                {
                    StopAllCoroutines();
                    spaceAS.Play();
                    StartCoroutine(DeactiveGameObjectInLoop(spaceAS,spaceASLoop));
                }
            break;
            case JumpType.Default:
                if (!spinJumpAS.isPlaying && !rLoopAS.isPlaying)
                {
                    StopAllCoroutines();
                    spinJumpAS.Play();
                    StartCoroutine(DeactiveGameObjectInLoop(spinJumpAS,rLoopAS));
                }
            break;
            case JumpType.Screw:
                if (!screwAS.isPlaying && !screwLoopAS.isPlaying)
                {
                    StopAllCoroutines();
                    screwAS.Play();
                    StartCoroutine(DeactiveGameObjectInLoop(screwAS,screwLoopAS));
                }
                break;
        }
    }
    private void DisableAudio(AudioSource loop,AudioSource start)
    {
        StopAllCoroutines();
        start.Stop();
        loop.Stop();
    }
    public void Balled()
    {
        StopAllCoroutines();
        StartCoroutine(DeactiveGameObject(balledAS));
    }
    public void BallJump()
    {
        StopAllCoroutines();
        StartCoroutine(DeactiveGameObject(morphjumpAS));
    }
    IEnumerator DeactiveGameObject(AudioSource audio){
        audio.Play();
        while(audio.isPlaying){
            yield return null;
        }
    }
    IEnumerator DeactiveGameObjectInLoop(AudioSource audio,AudioSource loop)
    {
        while (audio.isPlaying)
        {
            yield return null;
        }
        loop.Play();
        while(loop.isPlaying){
            yield return null;
        }
    }
    public void HyperJump()
    {
        StopAllCoroutines();
        StartCoroutine(DeactiveGameObject(hyperJumpAS));
    }

    public void StopAudio(StopAction stopAction){
        StopAllCoroutines();
        switch(stopAction){
            case StopAction.HyperJump:
            hyperJumpAS.Stop();
            break;
            case StopAction.All:
                StopAudios();
            break;
            case StopAction.Screw:
                StopAudio(JumpType.Screw);
            break;
        }
    }
    public void StopJumps(){
        StopAudio(JumpType.Screw);
        DisableAudio(screwLoopAS, screwAS);
        DisableAudio(spaceASLoop, spaceAS);
    }
    public void StopAudio(JumpType jumpType)
    {
        StopAllCoroutines();
        switch (jumpType)
        {
            case JumpType.Screw:
                DisableAudio(screwLoopAS, screwAS);
                DisableAudio(spaceASLoop, spaceAS);
                break;
            case JumpType.Space:
                DisableAudio(spaceASLoop, spaceAS);
                break;
            case JumpType.Default:
                DisableAudio(rLoopAS, spinJumpAS);
                break;
        }
    }
    private void StopAudios(){
        morphjumpAS.Stop(); 
        nJumpAS.Stop(); 
        balledAS.Stop(); 
        screwAS.Stop(); screwLoopAS.Stop(); 
        stepsAS.Stop(); 
        rLoopAS.Stop(); spinJumpAS.Stop();
        spaceAS.Stop();spaceASLoop.Stop();
        hyperJumpAS.Stop(); 
    }
}
public enum StopAction{
    All,HyperJump,Screw
}