using UnityEngine;

public class MoveDragHandle : DragHandle
{
    [SerializeField] protected RectTransform windowCanvas;
    [SerializeField] private RectTransform handleRect;
    [SerializeField] protected Sprite cursorSpriteOverride;

    private RectTransform windowRectTransform;
    
    private void OnEnable()
    {
        windowRectTransform = windowCanvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (cursorSpriteOverride && (isHovered || isDragging))
        {
            Cursor.Inst.SetCursorSpriteOverride(cursorSpriteOverride);
        }
        else //TODO: reset cursor override - probably need a system in Cursor for this
        {
            //Cursor.Inst.SetCursorSpriteOverride(null);
        }

        if(isDragging && windowCanvas)
        {
            windowCanvas.position += (Vector3)Cursor.Inst.PositionDelta_WS;
        }
    }
}
