using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDropData", menuName = "ItemDrop", order = 1)]
public class ItemDropData : ScriptableObject
{
    public ItemDrop[] Drops;
}

[System.Serializable]
public class ItemDrop
{
    public string Name;
    public ItemType Type;
    public int EffectID;
    public int Meshes;
    public Color[] Colors;
    public int Rarity;
}