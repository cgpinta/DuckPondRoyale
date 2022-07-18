using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    float timer;
    float oldtimer;
    private bool inProgress;
    public bool InProgress => inProgress;

    string name;

    public Timer()
    {
        timer = 0;
        oldtimer = 0;
        inProgress = false;
    }

    public Timer(string name)
    {
        timer = 0;
        oldtimer = 0;
        inProgress = false;
        this.name = name;
    }

    public void setTimer(float amount)
    {
        timer = amount;
        inProgress = true;
        Debug.Log(name+" set timer: " + timer);
    }
    public void updateTimer(float deltaTime)
    {
        if (timer > 0)
        {
            oldtimer = timer;
            //Debug.Log("flap timer: "+flapCooldownTimer);
            timer -= Time.deltaTime;
            //Debug.Log("timer: " + timer);
        }
        if (oldtimer > 0 && timer < 0)
        {
            oldtimer = 0;
            timer = 0;
            inProgress = false;
            Debug.Log("set to false");
        }
    }
}
