using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    //high-level cursor event enum for use by other scripts. Necessary, or use InputSystem somehow?
    [Flags]
    public enum CursorEvent
    {
        None = 0,
        MouseMove = 1 << 0,
        LeftClickDown = 1 << 1,
        LeftClickHold = 1 << 2,
        LeftClickUp = 1 << 3,
        RightClickDown = 1 << 4,
        RightClickHold = 1 << 5,
        RightClickUp = 1 << 6,
        MiddleClickDown = 1 << 7,
        MiddleClickHold = 1 << 8,
        MiddleClickUp = 1 << 9
    }

    [SerializeField] private PixelPerfectCamera pixelPerfectCamera;
    [SerializeField] private Image cursorImage;
    [SerializeField] private Sprite defaultCursorSprite;

    private Camera cam;

    //mouse raycasting
    private EventSystem eventSystem;
    [SerializeField] private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    //input
    private Vector2 mousePosition;
    private Vector2 clampedMousePos;

    private void OnEnable()
    {
        cam = pixelPerfectCamera.GetComponent<Camera>();
        eventSystem = FindFirstObjectByType<EventSystem>();
    }

    private void Update()
    {
        cursorImage.sprite = defaultCursorSprite;

        DoRaycast();
    }

    private void LeftClick()
    {
        Debug.Log("LeftClick");
        DoRaycast();
    }

    //TODO: make this more efficient!
    private void DoRaycast()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = clampedMousePos;

        raycastResults.Clear();
        //raycaster.Raycast(pointerEventData, raycastResults);
        eventSystem.RaycastAll(pointerEventData, raycastResults);

        foreach(var result in raycastResults)
        {
            if(result.gameObject.TryGetComponent<IOverrideCursorSprite>(out var cursorOverride))
            {
                DoCursorSpriteOverride(cursorOverride);
            }

            Debug.Log(result.gameObject.name);
        }
    }

    //TODO: refactor!!!!
    private void DoCursorSpriteOverride(IOverrideCursorSprite cursorOverride)
    {
        if(cursorOverride.OverrideOnInputEvent == CursorEvent.None || cursorOverride.OverrideOnInputEvent == CursorEvent.MouseMove)
        {
            if (cursorOverride.CursorSpriteOverride 
                && cursorImage.sprite != cursorOverride.CursorSpriteOverride)
            {
                cursorImage.sprite = cursorOverride.CursorSpriteOverride;
            }
        }
    }

    #region InputActions
    private void OnMouseMove(InputValue value)
    {
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
        if(value.isPressed)
        {
            LeftClick();
        }
    }
    
#endregion
}
