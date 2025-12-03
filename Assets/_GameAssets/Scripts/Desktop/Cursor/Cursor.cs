using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Cursor : SingletonMonoBehaviour<Cursor>
{
    //high-level cursor event enum for use by other scripts. Necessary, or use InputSystem somehow?
    [Flags]
    public enum CursorEvent
    {
        None = 0,
        MouseMove = 1 << 0,
        LeftClickDown = 1 << 1,
        LeftClickDrag = 1 << 2,
        LeftClickUp = 1 << 3,
        /*
        RightClickDown = 1 << 4,
        RightClickDrag = 1 << 5,
        RightClickUp = 1 << 6,
        MiddleClickDown = 1 << 7,
        MiddleClickDrag = 1 << 8,
        MiddleClickUp = 1 << 9 
        */
    }

    public struct SpriteOverride
    {
        public Sprite sprite;
        public int priority;
    }

    [SerializeField] private PixelPerfectCamera pixelPerfectCamera;
    [SerializeField] private Image cursorImage;
    [SerializeField, FormerlySerializedAs("defaultCursorSprite")] private Sprite defaultSprite;

    private Camera cam;

    //mouse raycasting
    private EventSystem eventSystem;
    [SerializeField] private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();
    private const int MaxRaycastListenerHits = 50;
    private ICursorEventListener[] listenerRaycastHits = new ICursorEventListener[MaxRaycastListenerHits];
    
    //input
    private CursorEvent currentEvent;
    private bool leftClickPressed;
    private Vector2 rawMousePosition;
    private Vector2 clampedRawMousePos;
    private Vector2 prevClampedMousePos_WS;

    //event listeners
    private HashSet<ICursorEventListener> trackedListeners = new HashSet<ICursorEventListener>();

    //event listeners the cursor is currently on top of
    private List<ICursorEventListener> hoveredListeners = new List<ICursorEventListener>();

    private List<SpriteOverride> spriteOverrides = new List<SpriteOverride>();

    public Vector2 ClampedMousePosition_WS => cam.ScreenToWorldPoint(clampedRawMousePos);

    public Vector2 PositionDelta_WS => ClampedMousePosition_WS - prevClampedMousePos_WS;

    private void OnEnable()
    {
        cam = pixelPerfectCamera.GetComponent<Camera>();
        eventSystem = FindFirstObjectByType<EventSystem>();

        UnityEngine.Cursor.visible = false; //hide default cursor (TODO: look at using Cursor.SetCursor instead?)
    }

    private void Update()
    {
        DoRaycast();

        if(currentEvent != CursorEvent.None)
        {
            Debug.Log(currentEvent);

            foreach (var listener in trackedListeners)
            {
                listener.OnCursorEvent(currentEvent);
            }
        }
    }

    private void LateUpdate()
    {
        currentEvent = CursorEvent.None;
        prevClampedMousePos_WS = ClampedMousePosition_WS;
    }

    //TODO: make this more efficient!
    private void DoRaycast()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = clampedRawMousePos;

        raycastResults.Clear();
        //raycaster.Raycast(pointerEventData, raycastResults);
        eventSystem.RaycastAll(pointerEventData, raycastResults);

        int listenerHitCount = 0;
        foreach (var result in raycastResults)
        {
            //DEBUG
            if(result.gameObject.TryGetComponent<DraggableUIElement>(out var draggable))
            {
                Debug.Log($"Hit {draggable.name} at {pointerEventData.position}");
            }
            
            //var hitListener = result.gameObject.GetComponentInParent<ICursorEventListener>(); //TODO: should look in parents, children, or only on the GameObject itself?
            //if(hitListener != null)
            if(result.gameObject.TryGetComponent<ICursorEventListener>(out var hitListener))
            {
                listenerRaycastHits[listenerHitCount] = hitListener;
                listenerHitCount++;
                
                //add to hovered elements
                if(trackedListeners.Contains(hitListener) && !hoveredListeners.Contains(hitListener))
                {
                    hoveredListeners.Add(hitListener);
                    hitListener.OnCursorEnter();
                }
            }
        }

        //check for elements the mouse is no longer hovering over
        for(int i = hoveredListeners.Count - 1; i >= 0; i--)
        {
            var listener = hoveredListeners[i];

            var hitByRaycast = false;
            for(int k = 0; k < listenerHitCount; k++)
            {
                if (listenerRaycastHits[k] == listener)
                {
                    hitByRaycast = true;
                    break;
                }
            }

            if(!hitByRaycast)
            {
                if(trackedListeners.Contains(listener))
                {
                    listener.OnCursorExit();
                }
                hoveredListeners.RemoveAt(i);
            }
        }
    }

    //TODO: refactor!!!! Use IOverrideCursorSprite interface and let cursor decide when/what to override?
    public void AddSpriteOverride(SpriteOverride spriteOverride)
    {
        if(spriteOverrides.Contains(spriteOverride))
        {
            return;
        }

        spriteOverrides.Add(spriteOverride);
        SetSpriteFromOverride(spriteOverride);
    }

    public void RemoveSpriteOverride(SpriteOverride spriteOverride)
    {
        spriteOverrides.Remove(spriteOverride);

        if(spriteOverrides.Count == 0)
        {
            cursorImage.sprite = defaultSprite;
        }
        else //use most recently-added override
        {
            SetSpriteFromOverride(spriteOverrides[spriteOverrides.Count - 1]);
        }
    }

    private void SetSpriteFromOverride(SpriteOverride spriteOverride)
    {
        if (spriteOverride.sprite)
        {
            cursorImage.sprite = spriteOverride.sprite;
        }
        else
        {
            Debug.LogError($"Cursor sprite override has null sprite!");
            cursorImage.sprite = Resources.Load<Sprite>("TX_Error_Sprite");
        }
    }

    public void AddCursorEventListener(ICursorEventListener listener)
    {
        if(!trackedListeners.Contains(listener))
        {
            trackedListeners.Add(listener);
            Debug.Log($"Adding cursor event listener {listener.ToString()};" +
                $" tracked listener listener count: {trackedListeners.Count}");
        }
    }

    public void RemoveCursorEventListener(ICursorEventListener listener)
    {
        Debug.Log($"Removing cursor event listener {listener.ToString()}; new listener count: {trackedListeners.Count - 1}");
        trackedListeners.Remove(listener);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if(cam)
        {
            Handles.Label(transform.position + (Vector3.right * 10f),
                $"World: {ClampedMousePosition_WS.x.ToString("0000")}, {ClampedMousePosition_WS.y.ToString("0000")} \n"
                + $"Raw: {rawMousePosition.x.ToString("0000")}, {rawMousePosition.y.ToString("0000")}");
        }
#endif
    }

    #region InputActions
    private void OnMouseMove(InputValue value)
    {
        if(leftClickPressed)
        {
            currentEvent = CursorEvent.LeftClickDrag;
        }
        else
        {
            currentEvent = CursorEvent.MouseMove;
        }
        
        var screenOffset = cam.transform.position - (new Vector3(Screen.width, Screen.height, 0f) / 2);
        
        //raw mouse position (i.e. Windows cursor position) is in range (0, 0) to (screen width, screen height),
        //from bottom-left to top-right
        rawMousePosition = value.Get<Vector2>();
        
        clampedRawMousePos = new Vector3(
            Mathf.Clamp(rawMousePosition.x, 0f, Screen.width),
            Mathf.Clamp(rawMousePosition.y, 0f, Screen.height));

        var worldPoint = cam.ScreenToWorldPoint(new Vector3(clampedRawMousePos.x, clampedRawMousePos.y, 0f));
        transform.position = new Vector3(worldPoint.x, worldPoint.y, 0f);
    }

    private void OnMouseLeftClick(InputValue value)
    {
        var floatVal = value.Get<float>();

        if(floatVal > 0f)
        {
            if(!leftClickPressed)
            {
                currentEvent = CursorEvent.LeftClickDown;
                leftClickPressed = true;
            }
        }
        else if(leftClickPressed)
        {
            currentEvent = CursorEvent.LeftClickUp;
            leftClickPressed = false;
        }
    }
    
#endregion
}
