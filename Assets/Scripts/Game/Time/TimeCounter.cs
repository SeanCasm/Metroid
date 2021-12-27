using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Time counter class only when player starts a new or loaded game.
/// </summary>
public class TimeCounter : MonoBehaviour
{
    public static TimeCounter instance;
    [SerializeField] TMPro.TextMeshProUGUI textMesh;
    public float time;
    private int ss, min, hr, dec;
    private void Awake()
    {
        instance = this;
    }

    private void OnDisable()
    {
        ResetTime();
    }

    private void ResetTime()
    {
        time = 0;
    }
    public void StartCounter()
    {
        textMesh.gameObject.SetActive(false);
        StartCoroutine("Counter");
    }
    public void PauseTimer()
    {
        StopCoroutine("Counter");
        textMesh.gameObject.SetActive(true);
        textMesh.text = TimeArrayIntToString();
    }
    IEnumerator Counter()
    {
        while (true)
        {
            time += Time.deltaTime;
            yield return null;
        }
    }
    private string TimeArrayIntToString()
    {
        string timeString;
        SetTimeValues();
        string[] newTime = AddCharAtBegin();
        timeString = newTime[0] + ":" + newTime[1] + ":" + newTime[2] + "." + newTime[3];
        return timeString;
    }
    public string TimeArrayIntToString(float time)
    {
        this.time = time;
        string timeString;
        SetTimeValues();
        string[] newTime = AddCharAtBegin();
        timeString = newTime[0] + ":" + newTime[1] + ":" + newTime[2] + "." + newTime[3];
        return timeString;
    }
    private string[] AddCharAtBegin()
    {
        string[] newTime = new string[4];
        newTime[0] = hr < 10 ? '0' + hr.ToString() : hr.ToString();
        newTime[1] = min < 10 ? '0' + min.ToString() : min.ToString();
        newTime[2] = ss < 10 ? '0' + ss.ToString() : ss.ToString();
        newTime[3] = dec < 10 ? '0' + dec.ToString() : dec.ToString();
        return newTime;
    }
    
    private void SetTimeValues()
    {
        int opt = (int)(time / 60);
        ss = (int)(time % 60);
        hr = (int)(opt / 60);
        min = (int)(opt % 60);
        dec = int.Parse(time.ToString().Split(',')[1].Substring(0, 2));
    }

}
