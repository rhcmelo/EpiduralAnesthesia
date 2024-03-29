﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float startTime;
    private bool finished;

    void Init()
    {        
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        startTime = Time.time;
        finished = false;
    }
    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (finished)
            return;

        if(timerText == null)
            timerText = GameObject.Find("TimerText").GetComponent<Text>();

        float t = Time.time - startTime;

        int minutes = ((int)t / 60);
        string sMinutes = minutes.ToString();
        string sSeconds = (t % 60).ToString("f2");

        timerText.text = sMinutes + ":" + sSeconds;

        if (minutes >= 5)
        {
            finished = true;
            timerText.color = Color.red;
        }
    }

    void Finnish()
    {
        finished = true;
        timerText.color = Color.red;
    }

    void Restart()
    {
        Init();
    }
}
