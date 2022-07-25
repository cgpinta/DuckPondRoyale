using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    float endTime;

    //public Timer()
    //{
    //    endTime = -1;
    //}

    public void setTimer(float amount)
    {
        endTime = Time.time + amount;
    }

    public bool isInProgress()
    {
        if(Time.time > endTime)
        {
            return false;
        }
        return true;
    }
}
