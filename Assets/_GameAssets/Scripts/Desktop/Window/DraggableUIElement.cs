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

                /* this warning is for me because it took me too long to figure this out
                Debug.LogWarning($"No camera set for this element's canvas! " +
                    $"(if there is no camera set, graphic raycasts on this element will " +
                    $"use screen- rather than world-space and so will raycast in the wrong place.");
                */
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

            if(!isHovered && OnHoverDragSprite)
            {
                Cursor.Inst.RemoveSpriteOverride(cursorSpriteOverride);
            }
        }
    }

    //ICursorEventListener
    public virtual void OnCursorEnter()
    {
        isHovered = true;
        if(OnHoverDragSprite)
        {
            Cursor.Inst.AddSpriteOverride(cursorSpriteOverride);
        }
    }

    //ICursorEventListener
    public virtual void OnCursorExit()
    {
        isHovered = false;
        if(!isDragging && OnHoverDragSprite)
        {
            Cursor.Inst.RemoveSpriteOverride(cursorSpriteOverride);
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
