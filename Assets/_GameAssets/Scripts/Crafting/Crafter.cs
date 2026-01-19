using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Crafter : SingletonMonoBehaviour<Crafter>, ICursorEventListener
{
    public enum CraftingResultState
    {
        NoIngredientMatch,
        PartialIngredientMatch,
        SuccessfulCraft
    }

    [SerializeField] private CraftingItemDatabase itemDatabase;
    [SerializeField] private CraftingItem thumbnailPrefab;
    [SerializeField] private CraftingItemWindow windowPrefab;
    [SerializeField] private CraftingItemData DEBUG_defaultItemData;
    [SerializeField] private List<CrafterPlacementZone> placementZones;

    private List<CraftingItemData> currentIngredients = new List<CraftingItemData>();

    private Dictionary<CraftingItem, HashSet<CraftingItem>> itemContacts = new Dictionary<CraftingItem, HashSet<CraftingItem>>();

    private bool canCraft;

    private void OnEnable()
    {
        foreach(var placementZone in placementZones)
        {
            placementZone.ItemPlaced += OnItemPlaced;
        }
    }

    private void Start()
    {
        Cursor.Inst.AddCursorEventListener(this);
    }

    private void OnDisable()
    {
        foreach (var placementZone in placementZones)
        {
            placementZone.ItemPlaced -= OnItemPlaced;
        }

        Cursor.Inst.RemoveCursorEventListener(this);
    }

    private void LateUpdate()
    {
        TryCraftAllItemContacts();

        var debugStr = "";
        foreach (var key in itemContacts.Keys)
        {
            debugStr += key.name + ": " + string.Join(", ", itemContacts[key]) + "\n";
        }
        Debug.Log(debugStr);
    }

    private void OnItemPlaced(CraftingItemData itemData)
    {
        Debug.Log($"Item {itemData.name} placed in crafter!");
        currentIngredients.Add(itemData);
    }

    public void AddItemContact(CraftingItem item, CraftingItem contactingItem)
    {
        if(itemContacts.TryGetValue(item, out var touchingItems))
        {
            if(!touchingItems.Contains(contactingItem))
            {
                touchingItems.Add(contactingItem);
            }
        }
        else
        {
            itemContacts.Add(item, new HashSet<CraftingItem> { item, contactingItem }); //item is always touching itself
        }
    }

    public void RemoveItemContact(CraftingItem item, CraftingItem contactingItem)
    {
        if(itemContacts.TryGetValue(item, out var touchingItems))
        {
            if(touchingItems.Contains(contactingItem))
            {
                touchingItems.Remove(contactingItem);
         
                //if(touchingItems.Count == 0)
                if(touchingItems.Count < 2) //delete if <2 since we always add the item itself to the touching items (TODO: this is jank)
                {
                    itemContacts.Remove(item);
                }
            }
            else
            {
                Debug.LogError($"No item {contactingItem.name} found in contacts list for item {item.name}!");
            }
        }
        else
        {
            Debug.LogError($"No key for item {item.name} found in contacts dictionary!");
        }
    }

    private CraftingResultState TryCraft(HashSet<CraftingItem> ingredients)
    {
        var resultState = itemDatabase.TryGetCraftResult(ingredients, out var craftResult);
        if(resultState == CraftingResultState.SuccessfulCraft)
        {
            //Instantiate new items
            InstantiateCraftingResult(craftResult, ingredients);
            foreach (var product in craftResult.ExtraProducts)
            {
                InstantiateCraftingResult(product, ingredients);
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
            ingredient.OnCraftAttempt(resultState);
        }

        return resultState;
    }

    //TODO: need to sort crafting flow out - really think about it!
    private void TryCraftAllItemContacts()
    {
        if (!canCraft) //TODO: prototype hack
        {
            return;
        }
        
        //ensure we don't craft duplicate ingredient sets TODO: allocation
        //DOUBLE TODO: ideally make a data structure to hold the contacting items graph which doesn't hold duplicates
        var usedIngredientSets = new List<HashSet<CraftingItem>>();
        var anySuccessfulCraft = false;
        foreach (var item in itemContacts.Keys)
        {
            var ingredients = itemContacts[item];
            foreach(var set in usedIngredientSets)
            {
                //already crafted with duplicate set
                if(ingredients.SetEquals(set))
                {
                    continue;
                }
            }

            var resultState = TryCraft(ingredients);
            if(resultState == CraftingResultState.SuccessfulCraft)
            {
                anySuccessfulCraft = true;
                usedIngredientSets.Add(ingredients);
            }
        }

        if(anySuccessfulCraft)
        {
            canCraft = false;
        }
    }

    private void InstantiateCraftingResult(CraftingItemData itemData, HashSet<CraftingItem> ingredients)
    {
        //TODO: PROTOTOTYPE
        var targetPos = Vector3.zero;
        foreach(var ingredient in ingredients)
        {
            targetPos += ingredient.transform.position;
        }
        targetPos /= ingredients.Count;
        targetPos += Vector3.up * 2f;

        var item = Instantiate<CraftingItem>(thumbnailPrefab, targetPos, Quaternion.identity);
        item.Data = itemData;
        item.OnCrafted();

        /*
        var window = Instantiate<CraftingItemWindow>(windowPrefab);
        window.SetItem(itemData);
        */
    }

    public void OnCursorEvent(Cursor.CursorEvent e)
    {
        //TODO: prototype hack
        if(e == Cursor.CursorEvent.LeftClickUp)
        {
            canCraft = true;
        }
    }

    private void OnDrawGizmos()
    {
        var contactPositions = new List<Vector3>();
        if(itemContacts.Count > 0)
        {
            Gizmos.matrix = Matrix4x4.identity;
            var hue = 0f;
            var inc = 1f / itemContacts.Count;
            foreach (var key in itemContacts.Keys)
            {
                contactPositions.Clear();
                var contacts = itemContacts[key];
                Gizmos.color = Color.HSVToRGB(hue, 1f, 1f);
                foreach (var contact in contacts)
                {
                    Gizmos.DrawSphere(contact.transform.position, 0.1f);
                    contactPositions.Add(contact.transform.position);
                }

                for(int i = 0; i < contactPositions.Count; i++)
                {
                    for(int j = 0; j < contactPositions.Count; j++)
                    {
                        if(i != j)
                        {
                            Gizmos.DrawLine(contactPositions[i], contactPositions[j]);
                        }
                    }
                }

                hue += inc;
            }
        }
    }
}
