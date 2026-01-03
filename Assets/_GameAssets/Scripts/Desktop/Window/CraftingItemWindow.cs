using UnityEngine;

public class CraftingItemWindow : Window
{
    public void SetItem(CraftingItemData itemData)
    {
        var windowContent = Instantiate<CraftingItemWindowContent>(itemData.WindowContent, ContentRoot);
        SetWindowContent(itemData.WindowContent);
    }
}
