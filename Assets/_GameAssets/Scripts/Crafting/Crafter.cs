using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafter : MonoBehaviour
{
    [SerializeField] private CraftingItemDatabase itemDatabase;
    [SerializeField] private CraftingItemThumbnail thumbnailPrefab;
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

    public bool TryCraft(List<CraftingItemData> ingredients)
    {
        if(itemDatabase.TryGetCraftResult(ingredients, out var result))
        {
            InstantiateCraftingResult(result);
            foreach (var product in result.ExtraProducts)
            {
                InstantiateCraftingResult(product);
            }

            //TODO: on successful craft, remove ingredients (and do -something- with them) 

            foreach (var placementZone in placementZones)
            {
                placementZone.RemoveItem();
            }

            return true;    
        }

        return false;
    }

    private void InstantiateCraftingResult(CraftingItemData itemData)
    {
        var mousePos = Cursor.Inst.ClampedPosition_WS;
        var item = Instantiate<CraftingItemThumbnail>(thumbnailPrefab, mousePos, Quaternion.identity);
        item.Data = itemData;

        var targetPos = mousePos + (Vector2)Random.onUnitSphere * Random.Range(100f, 400f);
        StartCoroutine(AnimateCraftResultCreation(item.Rect, targetPos));

        /*
        var window = Instantiate<CraftingItemWindow>(windowPrefab);
        window.SetItem(itemData);
        */
    }

    //TODO: prototype
    private IEnumerator AnimateCraftResultCreation(RectTransform item, Vector2 targetPos)
    {
        var startPos = item.position;

        var time = 0.2f;
        var t = 0f;
        while(t < 1f)
        {
            item.position = Vector3.Lerp(startPos, targetPos, t);
            t += Time.deltaTime / time;
            yield return null;
        }
    }

    //DEBUG
    [ContextMenu("Instantiate default craft result")]
    private void DEBUG_TestInstantiateCraftingResult()
    {
        InstantiateCraftingResult(DEBUG_defaultItemData);
    }
}
