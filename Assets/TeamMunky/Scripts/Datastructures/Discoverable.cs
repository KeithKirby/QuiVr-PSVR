public enum DiscoveryID
{
    Stitchz,
    Nutz,
    Splinterz,
    Coconutz,
    Ballz,
    Bullzeye,
    Shardz,
    Salutz,
    Cheapz,
    Tunez,
    Comboz,
    Toughz,
    Collateralz,
    Powerupz,
    Giantz,
    Buttz,

    Highz,
    Skillz,
    Batz,
    Hatz,
    Boomz,
    Chunkz,
    Bluez,
    Headz,
    Devoz,
    Gustz,
    Lightz,
    Bubbliez,
    Antiquez,
    Dropz,
    Tossz
}


public class Discoverable
{
    public readonly string DiscoverableName = "Name not set";    

    public bool HasBeenDiscovered = false;

    public readonly DiscoveryID ID;

    public Discoverable(string description, DiscoveryID ID)
    {
        DiscoverableName = description;
        this.ID = ID;
    }
}