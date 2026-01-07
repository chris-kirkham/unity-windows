using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    [SerializeField] private CraftingItemDatabase itemDatabase;
    [SerializeField] private CraftingItem thumbnailPrefab;
    [SerializeField] private CraftingItemWindow windowPrefab;
    [SerializeField] private CraftingItemData DEBUG_defaultItemData;
    [SerializeField] private List<CrafterPlacementZone> placementZones;

    private List<CraftingItemData> currentIngredients = new List<CraftingItemData>();

    private void OnEnable()
    {
        foreach(var placementZone in placementZones)
        {
            placementZone.ItemPlaced += OnItemPlaced;
        }
    }

    private void OnDisable()
    {
        foreach (var placementZone in placementZones)
        {
            placementZone.ItemPlaced -= OnItemPlaced;
        }
    }

    private void OnItemPlaced(CraftingItemData itemData)
    {
        Debug.Log($"Item {itemData.name} placed in crafter!");
        currentIngredients.Add(itemData);
    }

    public bool TryCraft(List<CraftingItem> ingredients)
    {
        var ingredientsData = new List<CraftingItemData>(ingredients.Count); //TODO: refactor
        foreach(var ingredient in ingredients)
        {
            ingredientsData.Add(ingredient.Data);
        }

        var successfulCraft = itemDatabase.TryGetCraftResult(ingredientsData, out var result);
        if(successfulCraft)
        {
            //Instantiate new items
            InstantiateCraftingResult(result);
            foreach (var product in result.ExtraProducts)
            {
                InstantiateCraftingResult(product);
            }

            /* TODO: figure out if using placement zones 
            foreach (var placementZone in placementZones)
            {
                placementZone.RemoveItem();
            }
            */
        }

        foreach (var ingredient in ingredients)
        {
            ingredient.OnUsedInCraft(successfulCraft);
        }

        return successfulCraft;
    }

    private void InstantiateCraftingResult(CraftingItemData itemData)
    {
        var mousePos = Cursor.Inst.ClampedPosition_WS;
        var item = Instantiate<CraftingItem>(thumbnailPrefab, mousePos, Quaternion.identity);
        item.Data = itemData;
        item.OnCrafted();

        /*
        var window = Instantiate<CraftingItemWindow>(windowPrefab);
        window.SetItem(itemData);
        */
    }

    

    //DEBUG
    [ContextMenu("Instantiate default craft result")]
    private void DEBUG_TestInstantiateCraftingResult()
    {
        InstantiateCraftingResult(DEBUG_defaultItemData);
    }
}
