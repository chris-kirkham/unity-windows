using System;
using UnityEngine;

public abstract class DraggablePlacementPoint : MonoBehaviour, ICursorEventListener
{
    [Flags]
    public enum PlacementSource
    { 
        Default = 1 << 0,
        PlayerDragAndDrop = 1 << 1,
        PlacementPointReturn = 1 << 2,
        Script = 1 << 3
    }

    [SerializeField] protected Cursor cursor;
    [SerializeField] protected bool returnable = true;

    protected virtual void OnEnable()
    {
        if(cursor)
        {
            Initialise(cursor);
        }
    }

    protected virtual void OnDisable()
    {
        if(cursor)
        {
            ((ICursorEventListener)this).DeregisterListener(cursor);
            cursor.DraggablesMgr.DragTargetChanged -= OnDragTargetChanged;
        }
    }

    public bool TryPlaceObject(DraggableObject obj, PlacementSource placementSource)
    {
        if (CanPlace(obj, placementSource))
        {
            //try remove object from its current placement, if any
            if(!obj.TryRemoveFromCurrentPlacementPoint())
            {
                return false;
            }

            PlaceObject(obj);
            Debug.Log($"Placed object {obj.name} onto {this.name}");
            obj.OnPlacedAtPlacementPoint(this); //TODO: UGH!! REFACTOR
            if (returnable)
            {
                obj.SetReturnPoint(this);
            }

            return true;
        }

        return false;
    }

    //Can the given object be placed on this placement point?
    protected abstract bool CanPlace(DraggableObject obj, PlacementSource placementSource);

    //place the object on this placement point
    protected virtual void PlaceObject(DraggableObject obj)
    {
    }

    public bool TryRemovePlacedObj(DraggableObject obj)
    {
        if (obj && CanRemovePlacedObj(obj))
        {
            OnPlacedObjRemoved(obj);
            Debug.Log($"Removed object {obj.name} from {this.name}");
            obj.OnRemovedFromPlacementPoint(this); //TODO: UGH: REFACTOR
            return true;
        }

        return false;
    }

    protected abstract bool CanRemovePlacedObj(DraggableObject obj);

    protected virtual void OnPlacedObjRemoved(DraggableObject obj)
    {
    }

    protected virtual void OnDraggableEnterPlacementArea(DraggableObject obj)
    {
        if (obj)
        {
            obj.AddHoveredPoint(this);
        }
    }

    protected virtual void OnDraggableExitPlacementArea(DraggableObject obj)
    {
        if (obj)
        {
            obj.RemoveHoveredPoint(this);
        }
    }

    public void Initialise(Cursor cursor)
    {
        this.cursor = cursor;
        ((ICursorEventListener)this).RegisterListener(cursor);
#if UNITY_EDITOR
        Debug.Log($"Registered {nameof(ICursorEventListener)} {name}");
#endif
        cursor.DraggablesMgr.DragTargetChanged += OnDragTargetChanged;
    }

    protected virtual void OnDragTargetChanged()
    {
        //if we just picked up a new drag target and are hovering over this element, treat it as if it just entered the placement area
        var dragTarget = cursor.CurrentDragTarget;
        if(dragTarget && cursor.IsHovered(this))
        {
            OnDraggableEnterPlacementArea(dragTarget);
        }
    }

    public virtual void OnCursorEvent(Cursor.EventID e) 
    {
        var dragTarget = cursor.CurrentDragTarget;
        if (dragTarget)
        {
            if (e == Cursor.EventID.EnterElement)
            {
                OnDraggableEnterPlacementArea(dragTarget);
            }
            else if (e == Cursor.EventID.ExitElement)
            {
                OnDraggableExitPlacementArea(dragTarget);
            }
        }
    }
}