using UnityEngine;

public interface ICursorEventListener
{
    public void OnCursorEnter();

    public void OnCursorExit();

    public void OnCursorEvent(Cursor.CursorEvent e);
}
