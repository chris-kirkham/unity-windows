using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    [SerializeField] private PixelPerfectCamera pixelPerfectCamera;

    private Camera cam;

    //mouse raycasting
    private EventSystem eventSystem;
    [SerializeField] private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    //input
    private Vector2 mousePosition;
    private Vector2 clampedMousePos;
    private Vector2 inputMousePos;

    private void OnEnable()
    {
        cam = pixelPerfectCamera.GetComponent<Camera>();
        eventSystem = FindFirstObjectByType<EventSystem>();

    }

    private void Update()
    {
        inputMousePos = Input.mousePosition;
    }

    private void LeftClick()
    {
        Debug.Log("LeftClick");
        DoClickRaycast();
    }

    private void DoClickRaycast()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = clampedMousePos;

        raycastResults.Clear();
        //raycaster.Raycast(pointerEventData, raycastResults);
        eventSystem.RaycastAll(pointerEventData, raycastResults);

        foreach(var result in raycastResults)
        {
            Debug.Log(result.gameObject.name);
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
