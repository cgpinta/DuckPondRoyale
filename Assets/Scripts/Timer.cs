using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    float endTime;


    bool isStopWatch;
    private float startTime;
    string moveType;

    public Timer() {}

    public Timer(bool stopWatch)
    {
        isStopWatch = stopWatch;
        moveType = "";
    }

    public void setTimer(float amount)
    {
        if (!isStopWatch)
        {
            endTime = Time.time + amount;
            startTime = Time.time;
        }
    }

    public void startWatch(string type)
    {
        if (isStopWatch)
        {
            startTime = Time.time;
            moveType = type;
        }
    }

    public float stopWatch()
    {
        if(startTime != 0)
        {
            float sTime = startTime;
            startTime = 0;
            return Time.time - sTime;
        }
        return 0;
    }

    public float getWatch()
    {
        if (isStopWatch)
        {
            return Time.time - startTime;
        }
        return 0;
    }
    public float getTimer()
    {
        if (!isStopWatch)
        {
            return Time.time - startTime;
        }
        return 0;
    }

    public string getType()
    {
        if (isStopWatch)
        {
            return moveType;
        }
        return null;
    }

    public bool isInProgress()
    {
        if (!isStopWatch)
        {
            if (Time.time > endTime)
            {
                return false;
            }
            return true;
        }
        else
        {
            if(startTime != 0)
            {
                return true;
            }
            return false;
        }
    }
}
