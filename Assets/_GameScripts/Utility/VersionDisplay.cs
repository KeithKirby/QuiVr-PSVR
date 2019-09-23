using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class VersionDisplay : MonoBehaviour {

    Text t;

    void Start()
    {
        t = GetComponent<Text>();
        if (t != null)
            t.text = "v " + Application.version;
    }
}
