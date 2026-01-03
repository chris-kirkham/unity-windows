using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingItemDatabase", menuName = "Crafting/Item Database")]
public class CraftingItemDatabase : ScriptableObject
{
    //list of all craftable items
    [SerializeField] private List<CraftingItemData> itemList; 

    //TODO: PROTOTYPE, optimise!!!!
    public bool TryGetCraftResult(List<CraftingItemData> ingredients, out CraftingItemData result)
    {
        var validResults = new List<CraftingItemData>(itemList);

        for(int i = validResults.Count - 1; i >= 0; i--)
        {
            if (!DoIngredientsMatchResult(validResults[i], ingredients))
            {
                validResults.RemoveAt(i);
            }
        }

        if(validResults.Count == 0)
        {
            result = null;
            return false;
        }

        if(validResults.Count > 1)
        {
            Debug.LogError($"More than one valid result for ingredients {string.Join(", ", ingredients)}! " +
                $"Returning first result.");
        }

        result = validResults[0];
        return true;
    }

    private bool DoIngredientsMatchResult(CraftingItemData result, List<CraftingItemData> ingredients)
    {
        var prerequisites = result.Prerequisites;

        //if item has no prerequisites, ignore it when crafting
        if (prerequisites == null || prerequisites.Count == 0)
        {
            return false;
        }

        //if ingredients and prerequisites are different in number, this can't be the right item 
        //(items must have exactly the right ingredients to craft)
        if (prerequisites.Count != ingredients.Count)
        {
            return false;
        }

        //check the current ingredients are exactly the same as the prerequisites for this item
        foreach (var prerequisite in prerequisites)
        {
            if (!ingredients.Contains(prerequisite))
            {
                return false;
            }
        }

        return true;
    }
}
