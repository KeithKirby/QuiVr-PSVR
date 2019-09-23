using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour {

    public ScrollRect window;
    public GameObject titlePrefab;
    List<TitleObject> Titles;
    int curTitle;

    #region Setup

    IEnumerator Start()
    {
        while(Cosmetics.Titles == null)
        {
            yield return true;
        }
        CollectTitles();
    }

    public void CollectTitles()
    {
        if(Titles != null)
        {
            foreach(var v in Titles)
            {
                Destroy(v.holder);
            }
        }
        Titles = new List<TitleObject>();
        AddTitle("");
        Titles[0].button.interactable = false;
        Titles[0].checkBox.SetActive(false);
        foreach(var v in Cosmetics.Titles)
        {
            AddTitle(v);
        }
        Titles.Sort((x, y) => x.titleText.text.CompareTo(y.titleText.text));
        if(Cosmetics.instance != null       && 
            Cosmetics.instance.TitleID > -1 && 
            Cosmetics.Titles.Count > Cosmetics.instance.TitleID)
        {
            SetTitle(Cosmetics.Titles[Cosmetics.instance.TitleID]);
        }
        SortObjects();
        for(int i=0; i<Titles.Count; i++)
        {
            Titles[i].button.onClick.AddListener(delegate { SelectTitle(i); });
        }
    }

    void AddTitle(string title)
    {
        GameObject v = Instantiate(titlePrefab, titlePrefab.transform.parent);
        //Setup Objects
        v.SetActive(true);
        TitleObject t = new TitleObject();
        t.holder = v;
        t.titleText = v.GetComponentInChildren<Text>();
        t.button = v.GetComponent<Button>();
        Image[] imgs = v.GetComponentsInChildren<Image>();
        foreach (var i in imgs)
        {
            if (i.gameObject != gameObject)
                t.checkBox = i.gameObject;
        }
        t.checkBox.SetActive(true);
        //Setup Values
        t.title = title;
        t.titleText.text = Cosmetics.ParseTitle(title, true);
        t.button.onClick.AddListener(delegate { SetTitle(title); });
        Titles.Add(t);
    }

    void SortObjects()
    {
        for(int i=0; i < Titles.Count; i++)
        {
            Titles[i].holder.transform.SetAsLastSibling();
        }
    }

    #endregion
    public void SetTitle(string t)
    {
        Cosmetics.SetTitle(t);
        foreach(var v in Titles)
        {
            if(v.title == t)
            {
                v.checkBox.SetActive(false);
                v.button.interactable = false;
            }
            else
            {
                v.checkBox.SetActive(true);
                v.button.interactable = true;
            }
        }
    }

    public void Reset()
    {
    }

    public void SelectTitle(int idx)
    {
        Invoke("ScrollToTop", 0.15f);
    }

    [AdvancedInspector.Inspect]
    public void ScrollToTop()
    {
        window.normalizedPosition = new Vector2(0, 1);
    }

    [System.Serializable]
    public class TitleObject
    {
        public GameObject holder;
        public Button button;
        public Text titleText;
        public GameObject checkBox;
        public string title;
        public bool available;
    }
}
