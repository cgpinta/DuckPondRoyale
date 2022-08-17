using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHolder
{
    public Animator Wings;
    public Animator Head;
    public Animator Body;
    public Animator Feet;

    public AnimatorHolder(Animator Wings, Animator Head, Animator Body, Animator Feet)
    {
        this.Wings = Wings;
        this.Head = Head;
        this.Body = Body;
        this.Feet = Feet;
    }

    public void SetIntForAll(string Trigger, int Value)
    {
        Wings.SetInteger(Trigger, Value);
        Head.SetInteger(Trigger, Value);
        Body.SetInteger(Trigger, Value);
        Feet.SetInteger(Trigger, Value);
    }
    public void SetFloatForAll(string Trigger, float Value)
    {
        Wings.SetFloat(Trigger, Value);
        Head.SetFloat(Trigger, Value);
        Body.SetFloat(Trigger, Value);
        Feet.SetFloat(Trigger, Value);
    }
    public void SetBoolForAll(string Trigger, bool State)
    {
        Wings.SetBool(Trigger, State);
        Head.SetBool(Trigger, State);
        Body.SetBool(Trigger, State);
        Feet.SetBool(Trigger, State);
    }
    public void SetTriggerForAll(string Trigger)
    {
        Wings.SetTrigger(Trigger);
        Head.SetTrigger(Trigger);
        Body.SetTrigger(Trigger);
        Feet.SetTrigger(Trigger);
    }
    public void ResetTriggerForAll(string Trigger)
    {
        Wings.ResetTrigger(Trigger);
        Head.ResetTrigger(Trigger);
        Body.ResetTrigger(Trigger);
        Feet.ResetTrigger(Trigger);
    }

    public void DisableAll()
    {
        Wings.enabled = false;
        Head.enabled = false;
        Body.enabled = false;
        Feet.enabled = false;
    }
    public void EnableAll()
    {
        Wings.enabled = true;
        Head.enabled = true;
        Body.enabled = true;
        Feet.enabled = true;
    }
}
