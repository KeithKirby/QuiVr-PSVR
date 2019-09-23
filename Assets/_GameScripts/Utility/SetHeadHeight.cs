using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHeadHeight : MonoBehaviour {

    [AdvancedInspector.Inspect]
	public void UpdateHeight()
    {
        Vector3 pos = transform.position;
        if(PlayerHead.instance != null)
        {
            pos.y = PlayerHead.instance.transform.position.y;
        }
        transform.position = pos;
    }

    public void UpdateDelayed()
    {
        StartCoroutine("UpdDelayed");
    }

    IEnumerator UpdDelayed()
    {
        yield return true;
        UpdateHeight();
    }
}
