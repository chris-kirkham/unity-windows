using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Window : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    //the root RectTransform under which all content displayed in the window should be added
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private PixelPerfectCamera pixelPerfectCam;
    [SerializeField] private Vector2 size;
    [SerializeField] private Vector2 minSize;
    [SerializeField] private Vector2 maxSize;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private WindowButton minimiseButton;
    [SerializeField] private WindowButton maximiseButton;
    [SerializeField] private WindowButton closeButton;
    [SerializeField] private Image icon;

    private WindowContent currentContent;

    public Vector2 MinSize => minSize;
    public Vector2 MaxSize => maxSize;

    public RectTransform ContentRoot => contentRoot;

    protected virtual void OnEnable()
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

    protected virtual void OnDisable()
    {
        if(currentContent)
        {
            currentContent.UnloadContent(this);
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

    protected virtual void OnValidate()
    {
        RefreshWindowSize();
    }

    public virtual void SetWindowContent(WindowContent content)
    {
        if(content == this.currentContent)
        {
            return;
        }

        if(currentContent)
        {
            currentContent.UnloadContent(this);
        }

        currentContent = content;
        content.LoadContent(this);
        SetSize(content.desiredWindowSize);
    }

    public void SetSize(Vector2 size)
    {
        this.size = size;
        RefreshWindowSize();
    }

    private void RefreshWindowSize()
    {
        if (canvasRect)
        {
            var newSize = pixelPerfectCam ? size / pixelPerfectCam.assetsPPU : size;
            canvasRect.sizeDelta = newSize;
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
