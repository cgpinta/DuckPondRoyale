using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float timer;
    float oldtimer;
    private bool inProgress;
    public bool InProgress => inProgress;

    public Timer()
    {
        timer = 0;
        oldtimer = 0;
        inProgress = false;
    }

    public void setTimer(float amount)
    {
        timer = amount;
        inProgress = true;
        Debug.Log("set timer: " + timer);
    }
    public void updateTimer(float deltaTime)
    {
        if (timer > 0)
        {
            oldtimer = timer;
            //Debug.Log("flap timer: "+flapCooldownTimer);
            timer -= Time.deltaTime;
            Debug.Log("timer: " + timer);
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
