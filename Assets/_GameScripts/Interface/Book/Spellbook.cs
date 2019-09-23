using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Spellbook : MonoBehaviour {

    BookControl bc;
    public BoxCollider OpenTrigger;
    public BoxCollider NextTrigger;
    public BoxCollider PrevTrigger;

    public UnityEvent OnOpen;
    public UnityEvent OnClose;
    public UnityEvent OnNextPage;
    public UnityEvent OnPrevPage;

    public bool isOpen;

    void Awake()
    {
        bc = GetComponent<BookControl>();
    }

    [AdvancedInspector.Inspect]
    public void Toggle()
    {
        Debug.Log("Toggling Book: " + gameObject.name);
        if (isOpen)
            PrevPage();
        else
            Open();
    }

	public void Open()
    {
        bc.Open_Book();
        isOpen = true;
        OnOpen.Invoke();
        Invoke("TurnPagesOn", 1.25f);
    }

    public void NextPage()
    {
        bc.Turn_Page();
        OnNextPage.Invoke();
    }

    public void PrevPage()
    {
        if (bc.CanGoBackAPage())
        {
            bc.Turn_BackPage();
            OnPrevPage.Invoke();
        }
        else
        {
            isOpen = false;
            bc.Close_Book();
            OnClose.Invoke();
            TogglePages(false);
            Invoke("EnableOpen", 1.25f);
        }
    }

    void EnableOpen()
    {
        if(OpenTrigger != null)
            OpenTrigger.enabled = true;
    }

    void TurnPagesOn()
    { TogglePages(true); }

    void TogglePages(bool on=true)
    {
        if(NextTrigger != null)
        {
            NextTrigger.enabled = on;
            PrevTrigger.enabled = on;
        }
    }
}
