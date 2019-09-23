static public class EnchantmentPublisher
{
    // An item has been placed in a chest
    public delegate void OnItemSelected(ItemChest chest, ArmorOption item, bool canUse);
    static public void PublishItemSelected(ItemChest chest, ArmorOption item, bool canUse) { ItemSelected.Invoke(chest, item, canUse); }
    static public event OnItemSelected ItemSelected;

    // Shards changed
    public delegate void OnShardsChanged();
    static public void PublishShardsChanged() { ShardsChanged.Invoke(); }
    static public event OnShardsChanged ShardsChanged;

    // Item disenchanged
    public delegate void OnItemDisenchanted(ArmorOption item);
    static public void PublishItemDisenchanted(ArmorOption item) { ItemDisenchanted.Invoke(item); }
    static public event OnItemDisenchanted ItemDisenchanted;
}
