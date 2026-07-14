using System;
using UnityEngine;


public interface ICursorEventListener
{
    public void RegisterListener(Cursor cursor)
    {
        if(cursor)
        {
            cursor.AddCursorEventListener(this);
        }
        else
        {
            Debug.LogError($"{nameof(Cursor)} is null! Cannot register listener.");
        }
    }

    public void DeregisterListener(Cursor cursor)
    {
        if(cursor)
        {
            cursor.RemoveCursorEventListener(this);
        }
        else
        {
            Debug.LogError($"{nameof(Cursor)} is null! Cannot deregister listener.");
        }
    }

    public void OnCursorEvent(Cursor.EventID e);
}
