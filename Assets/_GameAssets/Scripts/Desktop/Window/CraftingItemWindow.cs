using UnityEngine;

public class CraftingItemWindow : Window
{
    public void SetItem(CardData itemData)
    {
        var windowContent = Instantiate<CraftingItemWindowContent>(itemData.WindowContent, ContentRoot);
        SetWindowContent(itemData.WindowContent);
    }
}
