using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Window : MonoBehaviour
{
    [SerializeField] private PixelPerfectCamera pixelPerfectCam;
    [SerializeField] private Vector2Int pixelSize;
    [SerializeField] private Vector2 minSize;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform border;
    [SerializeField] private WindowButton minimiseButton;
    [SerializeField] private WindowButton maximiseButton;
    [SerializeField] private WindowButton closeButton;
    [SerializeField] private Image icon;
    [SerializeField] private RawImage contentHolder;

    private WindowContent content;

    public Vector2 MinSize => minSize;

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
        if(content)
        {
            content.UnloadContent();
        }

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

    public void SetContent(WindowContent content)
    {
        if(content != this.content)
        {
            this.content.UnloadContent();

            this.content = content;
            content.LoadContent();
            contentHolder.texture = content.GetContentTexture();
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
