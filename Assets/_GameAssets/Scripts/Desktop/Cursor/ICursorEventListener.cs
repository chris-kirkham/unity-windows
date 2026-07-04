using System;
using UnityEngine;

public interface ICursorEventListener
{ 

    protected Cursor ActiveCursor { get; set; }

    public void SetCursor(Cursor cursor)
    {
        ActiveCursor = cursor;
    }

    public void RegisterListener()
    {
        if(ActiveCursor)
        {
            ActiveCursor.AddCursorEventListener(this);
        }
        else
        {
            Debug.LogError($"No instance of {nameof(Cursor)} found! Cannot register listener.");
        }
    }

    public void DeregisterListener()
    {
        if(ActiveCursor)
        {
            ActiveCursor.RemoveCursorEventListener(this);
        }
    }

    public void OnCursorEvent(Cursor.EventID e);
}
