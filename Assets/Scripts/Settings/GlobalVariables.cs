using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
    static float swimInvincibility = 1;
    public static float getSwimInvincibility(){  return swimInvincibility;  }

    //public enum knockbackType
    //{
    //    Fixed,                  //The direction of knockback pushes away in the same direction no matter the direction of the hitbox owner
    //    Relative,               //The direction of knockback pushes away depending on the hitbox owner's direction
    //    Centered                //The direction of knockback pushes away from the center of the hitbox
    //}

    //public enum moveType
    //{
    //    Peck,
    //    Honk,
    //    Swim
    //}

}
