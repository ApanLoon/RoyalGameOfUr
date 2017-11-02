
using System;
using UnityEngine;

public class RNG
{
    public static int CurrentRoll;

    public static event Action OnRolled;

    public static void Roll()
    {
        CurrentRoll = UnityEngine.Random.Range(0, 4 + 1);

        if (OnRolled != null)
        {
            OnRolled();
        }
    }
}
