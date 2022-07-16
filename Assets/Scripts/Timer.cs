using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float timer = 0;
    float oldtimer = 0;
    bool inProgress = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer > 0)
        {
            oldtimer = timer;
            //Debug.Log("flap timer: "+flapCooldownTimer);
            timer -= Time.deltaTime;
        }
        if(oldtimer > 0 && timer < 0)
        {
            oldtimer = 0;
            timer = 0;
            inProgress = false;
        }
    }


    public void setTimer(float amount)
    {
        timer = amount;
        inProgress = true;
    }
    public bool isInProgress()
    {
        return inProgress;
    }
}
