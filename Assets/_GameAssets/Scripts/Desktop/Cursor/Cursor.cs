using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    private Vector2 mousePosition;
    private Vector2 clampedMousePos;
    private Vector2 prevClampedMousePos;

    //event listeners
    private HashSet<ICursorEventListener> trackedListeners = new HashSet<ICursorEventListener>();

    //event listeners the cursor is currently on top of
    private List<ICursorEventListener> hoveredListeners = new List<ICursorEventListener>();

    public Vector2 Position => mousePosition;

    public Vector2 PositionDelta => clampedMousePos - prevClampedMousePos;

    public Vector2 PositionDelta_WS => cam.ScreenToWorldPoint(clampedMousePos) - cam.ScreenToWorldPoint(prevClampedMousePos);

    private void OnEnable()
    {
        cam = pixelPerfectCamera.GetComponent<Camera>();
        eventSystem = FindFirstObjectByType<EventSystem>();
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
        prevClampedMousePos = clampedMousePos;
    }

    //TODO: make this more efficient!
    private void DoRaycast()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = clampedMousePos;

        raycastResults.Clear();
        //raycaster.Raycast(pointerEventData, raycastResults);
        eventSystem.RaycastAll(pointerEventData, raycastResults);

        int listenerHitCount = 0;
        foreach(var result in raycastResults)
        {
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
    public void SetCursorSpriteOverride(Sprite overrideSprite)
    {
        if(!overrideSprite)
        {
            cursorImage.sprite = defaultSprite;
        }
        else if (cursorImage.sprite != overrideSprite)
        {
            cursorImage.sprite = overrideSprite;
        }
    }

    public void AddCursorEventListener(ICursorEventListener listener)
    {
        if(!trackedListeners.Contains(listener))
        {
            trackedListeners.Add(listener);
        }
    }

    public void RemoveCursorEventListener(ICursorEventListener listener)
    {
        trackedListeners.Remove(listener);
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

        mousePosition = value.Get<Vector2>();
        clampedMousePos = new Vector3(
            Mathf.Clamp(mousePosition.x, 0f, Screen.width),
            Mathf.Clamp(mousePosition.y, 0f,Screen.height));

        if(pixelPerfectCamera)
        {
            var worldPoint = cam.ScreenToWorldPoint(new Vector3(clampedMousePos.x, clampedMousePos.y, 0f));
            transform.position = new Vector3(worldPoint.x, worldPoint.y, 0f);
        }
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
