using System;
using UnityEngine;

public abstract class MonoBehaviourBase : MonoBehaviour 
{
	protected void _Log(string format, params object[] args)
	{
        Debug.LogFormat(GetType().Name + ": " + format, args);
	}

    public I GetInterfaceComponent<I>(Component mb = null) where I : class
    {
        mb = mb == null ? this : mb;
        return mb.GetComponent(typeof(I)) as I;
    }
    public I[] GetInterfaceComponents<I>(Component mb = null) where I : class
    {
        mb = mb == null ? this : mb;
        Component[] components = mb.GetComponents(typeof(I));
        I[] ret = new I[components.Length];
        for (int i = 0; i < ret.Length; i++)
        {
            ret[i] = components[i] as I;
        }
        return ret;
    }
}