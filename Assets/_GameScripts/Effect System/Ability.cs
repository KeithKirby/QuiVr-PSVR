using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ability
{
    public string Name;
    public float Value;
    public float cost;

    public virtual void Execute()
    {

    }
}
