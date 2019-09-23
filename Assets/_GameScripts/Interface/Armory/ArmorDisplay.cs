using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorDisplay : MonoBehaviour {

    public string itemName;
    public ItemType itemType;
    public GameObject display;
    public Renderer rend;
    public Color ColorReplace = Color.black;
    public static List<ArmorDisplay> Displays;

    void Awake()
    {
        if (Displays == null)
            Displays = new List<ArmorDisplay>();
        Displays.Add(this);
    }

    public void UpdateAll()
    {
        if(Displays != null)
        {
            foreach(var v in Displays)
            {
                if (v != null)
                    v.CheckDisplay();
            }
        }
    }

    public virtual void CheckDisplay()
    {
        if (Armory.ValidFetch && Armory.instance != null)
        {
            ArmorOption[] opts = Armory.instance.GetOptions(itemType);
            foreach(var v in opts)
            {
                if(v.ItemName == itemName)
                {
                    display.SetActive(true);
                    if(rend != null && ColorReplace != Color.black)
                    {
                        float em = 4;
                        if (itemType == ItemType.ChestArmor || itemType == ItemType.Helmet)
                            em = 2.5f;
                        else if (itemType == ItemType.Gloves)
                            em = 3f;
                        rend.material.SetColor("_EmissionColor", ColorReplace * em);
                    }
                    return;
                }
            }
        }
        display.SetActive(false);
    }
}
