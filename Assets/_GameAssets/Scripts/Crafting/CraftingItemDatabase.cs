using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingItemDatabase", menuName = "Crafting/Item Database")]
public class CraftingItemDatabase : ScriptableObject
{
    //list of all craftable items
    [SerializeField] private List<CraftingItemData> itemList; 

    //TODO: PROTOTYPE, optimise!!!!
    public Crafter.CraftingResultState TryGetCraftResult(HashSet<CraftingItem> ingredients, out CraftingItemData result)
    {
        result = null;

        if (ingredients.Count < 2)
        {
            Debug.LogError($"Crafting attempted with <2 ingredients! This shouldn't happen");
            return Crafter.CraftingResultState.NoIngredientMatch;
        }

        //TODO: OPTIMISE!!!!!!!!
        foreach (var item in itemList)
        {
            if (item.Prerequisites.Count < 2)
            {
                continue;
            }

            int numMatching = 0;
            foreach (var prereqData in item.Prerequisites)
            {
                foreach (var ingredient in ingredients)
                {
                    if (ingredient.Data == prereqData)
                    {
                        numMatching++;
                    }
                }
            }

            if(numMatching == item.Prerequisites.Count)
            {
                result = item;
                return Crafter.CraftingResultState.SuccessfulCraft;
            }

            if (numMatching > 1)
            {
                return Crafter.CraftingResultState.PartialIngredientMatch;
            }
        }

        return Crafter.CraftingResultState.NoIngredientMatch;
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

    //TODO: Optimise!!!! Integrate with other function? This whole crafting-dictionary thing should use something other than lists
    public bool TryFindPartialIngredientMatch(HashSet<CraftingItem> ingredients)
    {
        if(ingredients.Count < 2)
        {
            return false;
        }

        //TODO: OPTIMISE!!!!!!!!
        foreach(var item in itemList)
        {
            if(item.Prerequisites.Count < 2)
            {
                continue;
            }

            int numMatching = 0;
            foreach(var prereqData in item.Prerequisites)
            {
                foreach(var ingredient in ingredients)
                {
                    if(ingredient.Data == prereqData)
                    {
                        numMatching++;
                    }
                }
            }

            //N.B. if we're checking for full matches anyhow this should be integrated into the other matching function
            if(numMatching > 1 && numMatching < item.Prerequisites.Count)
            {
                return true;
            }
        }

        return false;
    }
}
