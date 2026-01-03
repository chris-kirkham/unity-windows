using System;
using UnityEngine;

public class CrafterPlacementZone : MonoBehaviour, ICursorEventListener
{
    [SerializeField] private RectTransform zoneRect;

    public event Action<CraftingItemData> ItemPlaced;

    private bool isHovered;
    private bool itemPlaced;

    private CraftingItemThumbnail currentItem;

    private void Start()
    {
        if(Cursor.InstExists())
        {
            Cursor.Inst.AddCursorEventListener(this);
        }
    }

    private void OnDisable()
    {
        if(Cursor.InstExists())
        {
            Cursor.Inst.RemoveCursorEventListener(this);
        }
    }

    private void PlaceItem(CraftingItemThumbnail item)
    {
        itemPlaced = true;
        ItemPlaced?.Invoke(item.Data);
    }

    public void RemoveItem()
    {
        itemPlaced = false; 
    }

    private void StartPlacementPreview()
    {
        Debug.Log($"Starting placement preview for {currentItem.name}!");
    }

    private void StopPlacementPreview()
    {
        Debug.Log($"Stopping placement preview for {currentItem.name}!");
    }

    public void OnCursorEvent(Cursor.CursorEvent e)
    {
        if(e == Cursor.CursorEvent.EnterElement) //is cursor over this placement zone?
        {
            isHovered = true;
            
            if (Cursor.Inst.CurrentDragTarget 
                && Cursor.Inst.CurrentDragTarget.TryGetComponent<CraftingItemThumbnail>(out var item))
            {
                currentItem = item;
                StartPlacementPreview();
            }
        }
        else if(e == Cursor.CursorEvent.ExitElement)
        {
            isHovered = false;
            StopPlacementPreview();
            currentItem = null;
        }

        //if releasing a dragged crafting item over this placement zone
        if (e == Cursor.CursorEvent.LeftClickUp && currentItem) 
        {
            PlaceItem(currentItem);
        }
    }

    private Vector3[] gizmosRectCornersArray = new Vector3[4];
    private void OnDrawGizmos()
    {
        if(zoneRect)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = itemPlaced ? Color.cyan : isHovered ? Color.green : Color.white;
            zoneRect.GetWorldCorners(gizmosRectCornersArray);
            Gizmos.DrawLineStrip(gizmosRectCornersArray, looped: true);
        }
    }
}
