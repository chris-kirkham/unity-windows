using UnityEngine;
using UnityEngine.Serialization;

//base class for click-and-draggable UI items
public class DraggableUIElement : MonoBehaviour, ICursorEventListener
{
    [SerializeField, FormerlySerializedAs("windowCanvas")] protected Canvas canvas;
    [SerializeField] protected RectTransform handleRect;

    protected bool isHovered;
    protected bool isDragging;

    protected virtual Sprite OnHoverDragSprite { get; set; }

    protected Cursor.SpriteOverride cursorSpriteOverride; 
    

    protected virtual void Start()
    {
        cursorSpriteOverride = new Cursor.SpriteOverride()
        {
            sprite = OnHoverDragSprite
        };

        if (Cursor.InstExists())
        {
            Cursor.Inst.AddCursorEventListener(this);
        }
        else
        {
            Debug.LogError($"Instance of {nameof(Cursor)} not found!");
        }    

        if(canvas)
        {
            if(canvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogError("Canvas for this element is not world space! " +
                    "Draggable UI elements must use a world-space canvas.");
            }
            else if(!canvas.worldCamera)
            {
                var cam = FindFirstObjectByType<Camera>();
                if(cam)
                {
                    canvas.worldCamera = cam;
                    Debug.Log($"Using {cam.name} as event camera for {nameof(DraggableUIElement)} {name}'s Canvas.");
                }

                //this warning is for me because it took me too long to figure this out
                Debug.LogWarning($"No camera set for this element's canvas! " +
                    $"(if there is no camera set, graphic raycasts on this element will " +
                    $"use screen- rather than world-space and so will raycast in the wrong place.");
            }
        }
        else
        {
            Debug.LogError($"No Canvas set for {nameof(DraggableUIElement)} {name}! Dragging will not work properly.");
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

    private void LateUpdate()
    {
        //only remove as drag target at end of frame to allow other scripts to use it before then
        //TODO: messy, refactor
        if(!isDragging && Cursor.Inst.CurrentDragTarget == this)
        {
            Cursor.Inst.CurrentDragTarget = null;
        }
    }

    private void StartDrag()
    {
        isDragging = true;
        Cursor.Inst.CurrentDragTarget = this;
        OnStartDrag();
    }

    private void EndDrag()
    {
        isDragging = false;
        OnEndDrag();
        if (!isHovered)
        {
            Cursor.Inst.RemoveSpriteOverride(cursorSpriteOverride);
        }
    }

    protected virtual void OnStartDrag()
    {
    }

    protected virtual void OnEndDrag()
    {
    }

    //TODO: When over multiple drag handles, sometimes it does both at once (e.g. moves AND resizes)!!
    //Only do the "topmost" one - work out a priority/layer/Z system for raycasts
    
    //ICursorEventListener
    public virtual void OnCursorEvent(Cursor.CursorEvent e)
    {
        switch(e)
        {
            case Cursor.CursorEvent.EnterElement:
                isHovered = true;
                Cursor.Inst.AddSpriteOverride(cursorSpriteOverride);
                break;
            case Cursor.CursorEvent.ExitElement:
                isHovered = false;
                if (!isDragging)
                {
                    Cursor.Inst.RemoveSpriteOverride(cursorSpriteOverride);
                }
                break;
            case Cursor.CursorEvent.LeftClickDown:
                if(isHovered && !Cursor.Inst.CurrentDragTarget) //if element hovered and not currently dragging something
                {
                    StartDrag();
                }
                break;
            case Cursor.CursorEvent.LeftClickUp:
                EndDrag();
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (!handleRect)
        {
            return;
        }

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;
        var handleRectWorldCorners = new Vector3[4];
        handleRect.GetWorldCorners(handleRectWorldCorners);
        Gizmos.DrawLineStrip(new System.ReadOnlySpan<Vector3>(handleRectWorldCorners), looped: true);
        var size = handleRectWorldCorners[2] - handleRectWorldCorners[0];

        if(isDragging)
        {
            Gizmos.color = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.1f);
            Gizmos.DrawCube(handleRectWorldCorners[0] + size / 2, size);
        }
        else if(isHovered)
        {
            Gizmos.color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.1f);
            Gizmos.DrawCube(handleRectWorldCorners[0] + size / 2, size);
        }
    }
}
