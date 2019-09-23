using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Database", order = 1)]
public class DataValues : ScriptableObject
{
    public int[] ResourceValues;
    public float[] RarityPercents;
    public Color[] RarityColors;
    public Sprite ResourceIcon;
    public Sprite[] ItemIcons;
    public GameObject[] Helmets;
    public GameObject[] Arrows;
    public GameObject[] Chestplates;
    public GameObject[] Quivers;
    public GameObject[] Bows;
    public GameObject[] Gloves;
    public GameObject[] BowDetails;
    public ArtInfo[] Artists;
    public Wings[] WingOptions;
    
    [System.Serializable]
    public class ArtInfo
    {
        public string Artist;
        public List<GameObject> Works;

        public override string ToString()
        {
            return Artist;
        }
    }

    public GameObject GetItem(int id, ItemType type)
    {
        if (type == ItemType.Arrow && Arrows.Length > id)
            return Arrows[id];
        else if (type == ItemType.BowBase && Bows.Length > id)
            return Bows[id];
        else if (type == ItemType.Helmet && Helmets.Length > id)
            return Helmets[id];
        else if (type == ItemType.ChestArmor && Chestplates.Length > id)
            return Chestplates[id];
        else if (type == ItemType.Quiver && Quivers.Length > id)
            return Quivers[id];
        else if (type == ItemType.Gloves && Gloves.Length > id)
            return Gloves[id];
        else if (type == ItemType.BowDetail && BowDetails.Length > id)
            return BowDetails[id];
        return null;
    }

    public string GetArtist(GameObject item)
    {
        if(item != null)
        {
            foreach (var v in Artists)
            {
                if (v.Works != null && v.Works.Contains(item))
                    return v.Artist;
            }
        }
        return "";
    }

    public int GetWingID(Wings wing)
    {
        for(int i=0; i<WingOptions.Length; i++)
        {
            if (wing.WingMat == WingOptions[i].WingMat)
                return i;
        }
        return -1;
    }

    public int GetWingID(Material mat)
    {
        for (int i = 0; i < WingOptions.Length; i++)
        {
            if (mat == WingOptions[i].WingMat)
                return i;
        }
        return -1;
    }
}

[System.Serializable]
public class Wings
{
    public Material WingMat = null;

    public override string ToString()
    {
        if (WingMat != null)
            return WingMat.name;
        return "No Material";
    }
}
