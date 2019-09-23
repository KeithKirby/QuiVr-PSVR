using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedInspector;

[CreateAssetMenu(fileName = "Tiles", menuName = "TileSet", order = 1)]
public class TileValues : ScriptableObject
{
    public TransitionTile[] TransitionTiles;
    public TileSet Snow;
    public TileSet Desert;
    public TileSet Highlands;
    public TileSet Cave;
    public TileSet Void;
    public EncounterTile[] EventTiles;

    public RockMat[] RockMaterials;

    public TilePrefab GetTransition(EnvironmentType to, EnvironmentType from)
    {
        if (TransitionTiles.Length == 0)
            return null;
        foreach(var v in TransitionTiles)
        {
            if (v.To == to && v.From == from)
                return v.tile;
        }
        return null;
    }

    public TilePrefab[] GetTiles(EnvironmentType type, TileType ttype)
    {
        if (type == EnvironmentType.Snow)
            return GetSet(Snow, ttype);
        else if (type == EnvironmentType.Desert)
            return GetSet(Desert, ttype);
        else if (type == EnvironmentType.Highlands)
            return GetSet(Highlands, ttype);
        else if (type == EnvironmentType.Cave)
            return GetSet(Cave, ttype);
        else if (type == EnvironmentType.Void)
            return GetSet(Void, ttype);
        return GetSet(Snow, ttype);
    }

    TilePrefab[] GetSet(TileSet set, TileType type)
    {
        if (type == TileType.End)
            return set.EndCaps;
        else if (type == TileType.Link)
            return set.Links;
        return set.Gates;
    }

    public void TryRockReplace(MeshRenderer r, EnvironmentType type)
    {
        Material m = r.sharedMaterial;
        foreach(var v in RockMaterials)
        {
            if (v.matches(m))
            {
                Material nm = v.GetReplacement(type);
                if (nm != null)
                    r.sharedMaterial = nm;
                else
                    Debug.Log("Could not find material for rock of type " + type.ToString());
            }                
        }
    }

    public List<EncounterTile> usedTiles;
    public EncounterTile GetValidEncounters(TileManager.RandomValue rand)
    {
        List<EncounterTile> ValidTiles = new List<EncounterTile>();
        List<EncounterTile> GoodTiles = new List<EncounterTile>();
        int diff = 101;
        if (GameBase.instance != null && GameBase.instance.Difficulty > 0)
            diff = (int)GameBase.instance.Difficulty;
        foreach (var v in EventTiles)
        {
            if (v.MinDifficulty <= diff && v.Available && (v.MaxDifficulty == 0 || v.MaxDifficulty >= diff))
            {
                ValidTiles.Add(v);
                if (!usedTiles.Contains(v))
                    GoodTiles.Add(v);
            }
        }
        //Reset Used List if Completed
        if(usedTiles.Count >= ValidTiles.Count && ValidTiles.Count > 0)
        {
            EncounterTile lastUsed = usedTiles[usedTiles.Count - 1];
            usedTiles = new List<EncounterTile>();
            if(ValidTiles.Count > 1)
                usedTiles.Add(lastUsed);
            return GetValidEncounters(rand); //RECURSION -- CAREFUL
        }
        if(GoodTiles.Count > 0)
        {
            EncounterTile selected = GoodTiles[rand.Next(0, GoodTiles.Count)];
            usedTiles.Add(selected);
            return selected;
        }
        return null;
    }

    [AdvancedInspector.Inspect]
    void SetMissingMats()
    {
        for(int i=0; i<RockMaterials.Length; i++)
        {
            RockMat mat = RockMaterials[i];
            Material snowMat = mat.Snow;
#if UNITY_EDITOR
            if(snowMat != null)
            {
                string name = snowMat.name;
                if(mat.Sand == null)
                {
                    var Sands = UnityEditor.AssetDatabase.FindAssets(name + "_Sand");
                    foreach (var v in Sands)
                        mat.Sand = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(v), typeof(Material));
                }
                if(mat.Cave == null)
                {
                    var Caves = UnityEditor.AssetDatabase.FindAssets(name + "_Cave");
                    foreach (var v in Caves)
                        mat.Cave = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(v), typeof(Material));
                }
                if (mat.Lava == null)
                {
                    var Lavas = UnityEditor.AssetDatabase.FindAssets(name + "_Lava");
                    foreach (var v in Lavas)
                        mat.Lava = (Material)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(v), typeof(Material));
                }
            }
#endif
        }
    }
}

[System.Serializable]
public class RockMat
{
    public Material Snow;
    public Material Sand;
    public Material Moss;
    public Material Swamp;
    public Material Lava;
    public Material Cave;
    public Material Void;

    public bool matches(Material m)
    {
        return m == Sand || m == Snow || m == Moss || m==Swamp || m==Lava || m== Cave || m == Void;
    }

    public Material GetReplacement(EnvironmentType type)
    {
        if (type == EnvironmentType.Snow)
            return Snow;
        else if (type == EnvironmentType.Desert && Sand != null)
            return Sand;
        else if (type == EnvironmentType.Lava && Lava != null)
            return Lava;
        else if (type == EnvironmentType.Highlands && Moss != null)
            return Moss;
        else if (type == EnvironmentType.Cave && Cave != null)
            return Cave;
        else if (type == EnvironmentType.Void && Void != null)
            return Void;
        return Snow;
    }

    public override string ToString()
    {
        if(Snow != null)
            return Snow.name;
        return "Not Set";
    }
}

[System.Serializable]
[AdvancedInspector(true)]
public class TransitionTile
{
    public TilePrefab tile;
    public EnvironmentType To;
    public EnvironmentType From;

    public override string ToString()
    {
        return From.ToString() + " to " + To.ToString();
    }
}

[System.Serializable]
public class EncounterTile
{
    public string Name;
    public GameObject EventObject;
    public TileRarity Rarity;
    public int MinDifficulty = 100;
    public int MaxDifficulty = 0;
    public GameObject TileRequirement;
    public bool Available = true;

    public override string ToString()
    {
        if(Name != null)
            return Name;
        return "Encounter";
    }
}

public enum EnvironmentType
{
    Snow,
    Cave,
    Desert,
    Lava,
    Highlands,
    Olympus,
    Void
}

public enum TileEnvironment
{
    Snow = EnvironmentType.Snow,
    Cave = EnvironmentType.Cave,
    Desert = EnvironmentType.Desert,
    Highlands = EnvironmentType.Highlands,
    Void = EnvironmentType.Void
}

[System.Serializable]
public class TileSet
{
    [Collection]
    public TilePrefab[] Links;
    [Collection]
    public TilePrefab[] Gates;
    [Collection]
    public TilePrefab[] EndCaps;
}

[System.Serializable]
public class TilePrefab
{
    [Descriptor]
    public string Name;
    public GameObject Prefab;
    public TileType type;
    public int Direction; //0 = Forward 90/-90 turns
    public float Value; //In 100x100 tiles
    public TileRarity Rarity;
    public bool Unique;
    public float HeightChange;
    public EnvironmentType env;

    public override string ToString()
    {
        return Name;
    }
}

public enum TileType
{
    Link,
    Gate,
    End,
    Encounter
}

public enum TileRarity
{
    Common,
    Uncommon,
    Rare,
    UltraRare
}
