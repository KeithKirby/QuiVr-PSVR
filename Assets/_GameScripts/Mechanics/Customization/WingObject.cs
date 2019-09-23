using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingObject : MonoBehaviour {

    public SkinnedMeshRenderer[] Wing;
    public HandFade FadeRef;
    int setID;
    public bool mine;

    void Start()
    {
        InvokeRepeating("CheckWing", 0.5f, 0.491f);
        Material m = Wing[0].sharedMaterial;
        wingColor = m.GetColor("_wing_color");
        dustColor = m.GetColor("_dust_color");
    }

    void CheckWing()
    {
        if (mine && (Cosmetics.WingIDs == null || Cosmetics.instance.WingID < 0 || Cosmetics.instance.WingID > Cosmetics.WingIDs.Count || Cosmetics.WingIDs.Count == 0))
        { 
            if(setID != 0)
                EquipWings(0);
            return;
        }
        else if (mine && setID != Cosmetics.WingIDs[Cosmetics.instance.WingID])
            EquipWings(Cosmetics.WingIDs[Cosmetics.instance.WingID]);
    }

    float lastf;
    void Update()
    {
        if(FadeRef != null)
        {
            float fade = 1-FadeRef.GetOpacity();
            if(fade != lastf)
            {
                lastf = fade;
                CurWing = Color.Lerp(wingColor, Color.black, lastf);
                CurDust = Color.Lerp(dustColor, Color.black, lastf);
                foreach (var v in Wing)
                {
                    v.material.SetColor("_wing_color", CurWing);
                    v.material.SetColor("_dust_color", CurDust);
                }
            }
        }
    }

    public Color wingColor;
    public Color dustColor;
    public Color CurWing;
    public Color CurDust;
    public void EquipWings(int wingID)
    {
        setID = wingID;
        if (mine && PlayerSync.myInstance != null && PhotonNetwork.inRoom)
            PlayerSync.myInstance.EquipWings(wingID);
        Material m = ItemDatabase.v.WingOptions[0].WingMat;
        if (wingID >= 0 && wingID < ItemDatabase.v.WingOptions.Length)
            m = ItemDatabase.v.WingOptions[setID].WingMat;
        wingColor = m.GetColor("_wing_color");
        dustColor = m.GetColor("_dust_color");
        foreach (var v in Wing)
        {
            v.sharedMaterial = m;
        }
    }
}
