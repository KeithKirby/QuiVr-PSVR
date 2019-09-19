using UnityEngine;
using System.Collections;

public class DynamicPage : MonoBehaviour {

    Camera c;
    public Spellbook book;

    void Awake()
    {
        c = GetComponent<Camera>();
    }
    
    void Start()
    {
        InvokeRepeating("UpdatePage", 0.1f, 1f);
    }

    public void UpdatePage()
    {
        if(book.isOpen)
            c.Render();
    }
}
