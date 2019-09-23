using System.Collections.Generic;
using UnityEngine;

// There must only ever be one of these in the scene. Currently attached to the same object as GameManager.
public static class DiscoveryManager
{
    private static List<Discoverable> _discoverables = new List<Discoverable> {
            new Discoverable("Stitchz", DiscoveryID.Stitchz),
            new Discoverable("Nutz", DiscoveryID.Nutz),
            new Discoverable("Splinterz", DiscoveryID.Splinterz),
            new Discoverable("Coconutz", DiscoveryID.Coconutz),
            new Discoverable("Ballz", DiscoveryID.Ballz),
            new Discoverable("Bullzeye", DiscoveryID.Bullzeye),
            new Discoverable("Shardz", DiscoveryID.Shardz),
            new Discoverable("Salutz", DiscoveryID.Salutz),
            new Discoverable("Cheapz", DiscoveryID.Cheapz),
            new Discoverable("Tunez", DiscoveryID.Tunez),
            new Discoverable("Comboz", DiscoveryID.Comboz),
            new Discoverable("Toughz", DiscoveryID.Toughz),
            new Discoverable("Collateralz", DiscoveryID.Collateralz),
            new Discoverable("Powerupz", DiscoveryID.Powerupz),
            new Discoverable("Giantz", DiscoveryID.Giantz),
            new Discoverable("Buttz", DiscoveryID.Buttz),

            new Discoverable("Highz", DiscoveryID.Highz),
            new Discoverable("Skillz", DiscoveryID.Skillz),
            new Discoverable("Batz", DiscoveryID.Batz),
            new Discoverable("Hatz", DiscoveryID.Hatz),
            new Discoverable("Boomz", DiscoveryID.Boomz),
            new Discoverable("Chunkz", DiscoveryID.Chunkz),
            new Discoverable("Bluez", DiscoveryID.Bluez),
            new Discoverable("Headz", DiscoveryID.Headz),
            new Discoverable("Devoz", DiscoveryID.Devoz),
            new Discoverable("Gustz", DiscoveryID.Gustz),
            new Discoverable("Lightz", DiscoveryID.Lightz),
            new Discoverable("Bubbliez", DiscoveryID.Bubbliez),
            new Discoverable("Antiquez", DiscoveryID.Antiquez),
            new Discoverable("Dropz", DiscoveryID.Dropz),
            new Discoverable("Tossz", DiscoveryID.Tossz)
        };

    /// <summary>
    /// What achievements have we already discovered? Particuarly useful for the Scoreboard.
    /// </summary>
    public static List<Discoverable> DiscoveredAchievements()
    {
        var discoveredAchievements = new List<Discoverable>();
        foreach (Discoverable discovery in _discoverables)
        {
            if (discovery.HasBeenDiscovered)
            {
                discoveredAchievements.Add(discovery);
            }
        }
        return discoveredAchievements;
    }

    /// <summary>
    /// Prepare DiscoveryManager to discover all achievements again, usually for creating new games. 
    /// </summary>
    public static void Reset()
    {
        foreach (Discoverable discovery in _discoverables)
        {
            discovery.HasBeenDiscovered = false;
        }
    }

    public static void setDiscoverables(int index)
    {
        Discoverable discovery = _discoverables[index];
        discovery.HasBeenDiscovered = true;
    }

    /// <summary>
    /// "Unlocks" an "Achievement"/discovery and plays a noise, unless it was previously discovered.
    /// </summary>
    public static void DiscoveryFound(DiscoveryID ID)
    {
        foreach (Discoverable discovery in _discoverables)
        {
            if (discovery.ID == ID)
            {
                if (discovery.HasBeenDiscovered == false)
                {
                    //SoundManager.instance.PlaySound(SoundManager.SoundType.CrowdReaction, Vector3.zero);
                    //soundManager.PlaySound(SoundManager.SoundType.PlayerKO, root.transform);
                    discovery.HasBeenDiscovered = true;
                    if (null != Discovered)
                        Discovered(ID);
                }
                return;
            }
        }
    }

    public delegate void OnDiscovered(DiscoveryID id);
    static public event OnDiscovered Discovered;
}
