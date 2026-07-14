using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingEventTracker
{
    private HashSet<CardData> uniqueItemsCrafted;
    private List<CardData> uniqueItemsCraftedInCraftOrder;

    public event Action<CardData> itemFirstCrafted;
    public event Action<CardData> onItemCrafted;

    public CraftingEventTracker()
    {
        uniqueItemsCrafted = new HashSet<CardData>();
        uniqueItemsCraftedInCraftOrder = new List<CardData>();
    }

    public void OnItemCrafted(CardData itemData)
    {
        if(!uniqueItemsCrafted.Contains(itemData))
        {
            uniqueItemsCrafted.Add(itemData);
            uniqueItemsCraftedInCraftOrder.Add(itemData);
            itemFirstCrafted?.Invoke(itemData);
        }

        onItemCrafted?.Invoke(itemData);
    }

    public List<CardData> GetUniqueItemsCrafted()
    {
        return uniqueItemsCraftedInCraftOrder;
    }

    public bool WasItemCraftedPreviously(CardData itemData)
    {
        return uniqueItemsCrafted.Contains(itemData);
    }
}
