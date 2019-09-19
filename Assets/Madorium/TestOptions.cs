using AdvancedInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOptions : MonoBehaviour, IDataChanged
{
    [AdvancedInspector.Inspect]
    public bool InvulnerablePlayer = false;
    [AdvancedInspector.Inspect]
    public bool InvulnerableGate = false;
    [AdvancedInspector.Inspect]
    [Range(1f, 100f)]
    public float DamageMultiplier = 1;

    [HideInInspector]
    static public float SDamageMultiplier = 1;
    [HideInInspector]
    static public bool SInvulnerablePlayer = false;
    [HideInInspector]
    static public bool SInvulnerableGate = false;

    public event GenericEventHandler OnDataChanged;

    public void DataChanged()
    {
        SDamageMultiplier = DamageMultiplier;
        SInvulnerablePlayer = InvulnerablePlayer;
        SInvulnerableGate = InvulnerableGate;
    }

    // Use this for initialization
    void Start ()
    {
        DataChanged();
    }
}
