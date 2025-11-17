using UnityEngine;

//base class for click-and-drag stuff
public class DragHandle : MonoBehaviour, ICursorEventListener
{
    protected bool isHovered;
    protected bool isDragging;

    protected virtual void Start()
    {
        if (Cursor.InstExists())
        {
            Cursor.Inst.AddCursorEventListener(this);
        }
        else
        {
            Debug.LogError($"Instance of {nameof(Cursor)} not found!");
        }    
    }

    protected virtual void OnDisable()
    {
        if (Cursor.InstExists())
        {
            Cursor.Inst.RemoveCursorEventListener(this);
        }
        else
        {
            Debug.LogError($"Instance of {nameof(Cursor)} not found!");
        }
    }

    protected virtual void OnStartDrag()
    {
    }

    protected virtual void OnEndDrag()
    {
    }

    //ICursorEventListener
    public virtual void OnCursorEvent(Cursor.CursorEvent e)
    {
        if (isHovered)
        {
            if (e == Cursor.CursorEvent.LeftClickDown)
            {
                isDragging = true;
                OnStartDrag();
            }
        }

        if (e == Cursor.CursorEvent.LeftClickUp)
        {
            isDragging = false;
            OnEndDrag();
        }
    }

    //ICursorEventListener
    public virtual void OnCursorEnter()
    {
        isHovered = true;
    }

    //ICursorEventListener
    public virtual void OnCursorExit()
    {
        isHovered = false;
    }
}
