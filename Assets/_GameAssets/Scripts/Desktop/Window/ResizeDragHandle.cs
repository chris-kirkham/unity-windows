using NUnit.Framework;
using System;
using UnityEngine;

public class ResizeDragHandle : MonoBehaviour, ICursorEventListener, IOverrideCursorSprite
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

    private bool isHovered;
    private bool isDragging;
    private Vector2 dragStartPos;

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

    private Vector2 prevCursorPos;
    
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

    private void OnEnable()
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

    private void Update()
    {
        if(isHovered && cursorSpriteOverride)
        {
            Cursor.Inst.SetCursorSpriteOverride(this);
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
                    newHeight = canvasRect.height - mouseDelta.y;
                    break;
                case Side.Bottom:
                    newHeight = canvasRect.height + mouseDelta.y;
                    break;
                default:
                    break;
            }

            windowCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            windowCanvas.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            canvasRect.center = Vector2.zero; //TEST
        }
    }


    //ICursorEventListener
    public void OnCursorEvent(Cursor.CursorEvent e)
    {
        if(isHovered)
        {
            if (e == Cursor.CursorEvent.LeftClickDown)
            {
                isDragging = true;
                
                //TODO: adjust pivot for each side and reposition window properly
                var prevPivot = windowCanvas.pivot;
                windowCanvas.pivot = resizePivots[(int)side];
                //windowCanvas.position *= windowCanvas.pivot - prevPivot;
                
                dragStartPos = Cursor.Inst.Position;
            }
        }

        if (e == Cursor.CursorEvent.LeftClickUp)
        {
            isDragging = false;
        }
    }

    public void OnCursorEnter()
    {
        Debug.Log($"Drag handle {name} entered!");
        isHovered = true;
    }

    public void OnCursorExit()
    {
        Debug.Log($"Drag handle {name} exited!");
        isHovered = false;
    }

    //IOverrideCursorSprite
    public Sprite CursorSpriteOverride => cursorSpriteOverride;
    public Cursor.CursorEvent OverrideOnInputEvent => Cursor.CursorEvent.MouseMove;

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
