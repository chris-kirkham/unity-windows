using NUnit.Framework;
using System;
using UnityEngine;

//TODO: all the drag-resizing code can probably be made much neater
public class ResizeDragHandle : DragHandle
{
    private enum Side
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Bottom = 3,
        TopLeft = 4,
        TopRight = 5,
        BottomLeft = 6,
        BottomRight = 7
    }

    [SerializeField] private Side side;
    [SerializeField] private RectTransform windowCanvas;
    [SerializeField] private RectTransform handleRect;
    [SerializeField] private Sprite cursorSpriteOverride;

    //drag resize pivots
    //(0, 0) == bottom left
    //These pivots are the opposite to the side of the drag bars, so the canvas will move "with" the drag bar 
    private readonly Vector2 resizePivot_Left = new Vector2(1f, 0.5f);
    private readonly Vector2 resizePivot_Right = new Vector2(0f, 0.5f);
    private readonly Vector2 resizePivot_Top = new Vector2(0.5f, 0f);
    private readonly Vector2 resizePivot_Bottom = new Vector2(0.5f, 1f);
    private readonly Vector2 resizePivot_TopLeft = new Vector2(1f, 0f);
    private readonly Vector2 resizePivot_TopRight = new Vector2(0f, 0f);
    private readonly Vector2 resizePivot_BottomLeft = new Vector2(1f, 1f);
    private readonly Vector2 resizePivot_BottomRight = new Vector2(0f, 0f);
    private Vector2[] resizePivots;

    private void Awake()
    {
        resizePivots = new Vector2[]
        {
            resizePivot_Left,
            resizePivot_Right,
            resizePivot_Top,
            resizePivot_Bottom,
            resizePivot_TopLeft,
            resizePivot_TopRight,
            resizePivot_BottomLeft,
            resizePivot_BottomRight
        };
    }

    private void Update()
    {
        if(cursorSpriteOverride && (isHovered || isDragging))
        {
            Cursor.Inst.SetCursorSpriteOverride(cursorSpriteOverride);
        }
        else //TODO: reset cursor override - probably need a system in Cursor for this
        {
            //Cursor.Inst.SetCursorSpriteOverride(null);
        }

        if(isDragging && windowCanvas)
        {
            var canvasRect = windowCanvas.rect;
            var mouseDelta = Cursor.Inst.PositionDelta_WS;
            var newWidth = canvasRect.width;
            var newHeight = canvasRect.height;
            var prevPivot = windowCanvas.pivot;

            switch(side)
            {
                case Side.Left:
                    newWidth = canvasRect.width - mouseDelta.x;
                    break;
                case Side.Right:
                    newWidth = canvasRect.width + mouseDelta.x;
                    break;
                case Side.Top:
                    newHeight = canvasRect.height + mouseDelta.y;
                    break;
                case Side.Bottom:
                    newHeight = canvasRect.height - mouseDelta.y;
                    break;
                case Side.TopLeft:
                    newWidth = canvasRect.width - mouseDelta.x;
                    newHeight = canvasRect.height + mouseDelta.y;
                    break;
                case Side.TopRight:
                    newWidth = canvasRect.width + mouseDelta.x;
                    newHeight = canvasRect.height + mouseDelta.y;
                    break;
                case Side.BottomLeft:
                    newWidth = canvasRect.width - mouseDelta.x;
                    newHeight = canvasRect.height - mouseDelta.y;
                    break;
                case Side.BottomRight:
                    newWidth = canvasRect.width + mouseDelta.x;
                    newHeight = canvasRect.height - mouseDelta.y;
                    break;
                default:
                    break;
            }

            windowCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            windowCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }
    }

    protected override void OnStartDrag()
    {
        //adjust pivot for each side and reposition window properly
        var prevPivot = windowCanvas.pivot;
        windowCanvas.pivot = resizePivots[(int)side];
        var pivotDelta = windowCanvas.pivot - prevPivot;
        var newPos = windowCanvas.position + (Vector3)(windowCanvas.rect.size * pivotDelta);
        windowCanvas.position = newPos;
    }

    private void OnDrawGizmos()
    {
        if(!handleRect)
        {
            return;
        }

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;
        var handleRectWorldCorners = new Vector3[4];
        handleRect.GetWorldCorners(handleRectWorldCorners);
        Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(handleRectWorldCorners), looped: true);
        var size = handleRectWorldCorners[2] - handleRectWorldCorners[0];

        Gizmos.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.1f);
        Gizmos.DrawCube(handleRectWorldCorners[0] + size / 2, size);
    }
}
