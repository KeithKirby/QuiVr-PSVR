using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[RequireComponent (typeof(Button))]
public class GMenuButton : MonoBehaviour {

    Button b;
    public float lastPress;

    void Awake()
    {
        b = GetComponent<Button>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Pointer") && Time.unscaledTime - lastPress > 0.5f)
        {
            b.onClick.Invoke();
            lastPress = Time.unscaledTime;
        }
    }
}
