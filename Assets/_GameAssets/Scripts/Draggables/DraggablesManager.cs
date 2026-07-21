using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DraggablesManager
{
    private HashSet<DraggableObject> dragRequests = new HashSet<DraggableObject>();

    public DraggableObject CurrentDragTarget 
    {
        get => currentDragTarget;
        private set
        {
            prevDragTarget = CurrentDragTarget;
            currentDragTarget = value;

            if(prevDragTarget != currentDragTarget)
            {
                DragTargetChanged?.Invoke();
            }
        }
    }

    public HashSet<DraggableObject> DragRequests => dragRequests;

    private Cursor cursor;
    private DraggableObject currentDragTarget;
    private DraggableObject prevDragTarget;

    public event Action DragTargetChanged;

    public void SetCursor(Cursor cursor)
    {
        this.cursor = cursor;
    }

    public void RequestDrag(DraggableObject draggable)
    {
        dragRequests.Add(draggable);
    }

    public void EndDrag(DraggableObject draggable)
    {
        dragRequests.Remove(draggable);
    }

    public void UpdateDragTarget()
    {
        if (CurrentDragTarget)
        {
            //if we're already dragging something in the drag list, keep dragging that
            if (dragRequests.Contains(CurrentDragTarget))
            {
                return;
            }
            else //if current drag target was removed from the drag list, end that drag (TODO: hmm)
            {
                CurrentDragTarget = null;
            }
        }

        if (dragRequests.Count == 0)
        {
            return;
        }

        var bestDraggable = FindBestDragTarget();
        if (bestDraggable)
        {
            if(bestDraggable.TryStartDrag())
            {
                CurrentDragTarget = bestDraggable;
            }
            else
            {
                dragRequests.Remove(bestDraggable);

                if(dragRequests.Count == 0)
                {
                    Debug.LogError("There were drag requests, but no valid drag target found among them!");
                }
                else
                {
                    UpdateDragTarget(); //try find a drag target among the remaining drag requests
                }
            }
        }
    }

    private DraggableObject FindBestDragTarget()
    {
        //find best drag option by y distance to camera - TODO: think of a smarter way to do this
        DraggableObject bestDraggable = null;
        var minDist = Mathf.Infinity;
        foreach (var draggable in dragRequests)
        {
            if (!draggable)
            {
                continue;
            }

            var dist = GetDraggableScore(draggable.transform);
            if (dist < minDist)
            {
                bestDraggable = draggable;
                minDist = dist;
            }
        }

        return bestDraggable;
    }

    //Lower the better, like in golf!
    public float GetDraggableScore(Transform draggable)
    {
        return Mathf.Abs(draggable.position.y - cursor.Cam.transform.position.y);
    }
}
