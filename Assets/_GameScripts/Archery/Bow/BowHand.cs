using UnityEngine;
using System.Collections;

public class BowHand : MonoBehaviour {

    Vector3 startPos;

    IEnumerator Start()
    {
        yield return true;
        startPos = transform.position;
    }
}
