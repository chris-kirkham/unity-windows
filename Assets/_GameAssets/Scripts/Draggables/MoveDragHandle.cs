using UnityEngine;
using UnityEngine.Serialization;

public class MoveDragHandle : DraggableUIElement
{
    [SerializeField] protected Sprite cursorOverride;

    protected override Sprite OnHoverDragSprite => cursorOverride;

    private RectTransform canvasRect;

    private void Awake()
    {
        if (canvas)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if(isDragging && canvasRect)
        {
            canvasRect.position += (Vector3)Cursor.Inst.ClampedPositionDelta_WS;
        }
    }
}
