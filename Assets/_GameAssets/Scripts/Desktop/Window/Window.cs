using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

public class Window : MonoBehaviour
{
    [SerializeField] private PixelPerfectCamera pixelPerfectCam;
    [SerializeField] private Vector2Int pixelSize;
    [SerializeField] private Canvas canvas;
    [SerializeField, FormerlySerializedAs("canvas")] private RectTransform canvasRect;
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform border;
    [SerializeField] private WindowButton minimiseButton;
    [SerializeField] private WindowButton maximiseButton;
    [SerializeField] private WindowButton closeButton;
    [SerializeField] private Image icon;

    private void OnEnable()
    {
        if(canvas && pixelPerfectCam)
        {
            canvas.worldCamera = pixelPerfectCam.GetComponent<Camera>();
        }

        if(minimiseButton)
        {
            minimiseButton.ButtonPressed += OnMinimiseButtonClicked;
        }

        if(maximiseButton)
        {
            maximiseButton.ButtonPressed += OnMaximiseButtonClicked;
        }

        if(closeButton)
        {
            closeButton.ButtonPressed += OnCloseButtonClicked;
        }
    }

    private void OnDisable()
    {
        if (minimiseButton)
        {
            minimiseButton.ButtonPressed -= OnMinimiseButtonClicked;
        }

        if (maximiseButton)
        {
            maximiseButton.ButtonPressed -= OnMaximiseButtonClicked;
        }

        if (closeButton)
        {
            closeButton.ButtonPressed -= OnCloseButtonClicked;
        }
    }

    private void OnValidate()
    {
        if (canvasRect && pixelPerfectCam)
        {
            canvasRect.sizeDelta = pixelSize / pixelPerfectCam.assetsPPU;
        }
    }

    private void OnMinimiseButtonClicked()
    {
        Debug.Log("Minimised!");
    }

    private void OnMaximiseButtonClicked()
    {
        Debug.Log("Maximised!");
    }

    private void OnCloseButtonClicked()
    {
        Debug.Log("Closed!");
    }
}
