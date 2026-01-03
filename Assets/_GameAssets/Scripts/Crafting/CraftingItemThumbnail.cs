using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class CraftingItemThumbnail : MonoBehaviour, ICursorEventListener
{
    [SerializeField] private CraftingItemData itemData;
    
    [Header("UI")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RawImage thumbnailImage;
    //[SerializeField] private Texture2D thumbnailTex;
    [SerializeField] private Vector2 size = new Vector2(100f, 100f);
    [SerializeField] private bool useThumbnailSize = true;
    [SerializeField] private float thumbnailScale = 0.25f;

    private bool isHovered;

    public CraftingItemData Data 
    {
        get => itemData;
        set
        {
            itemData = value;
            UpdateData();
        } 
    } 

    public Vector2 Size => useThumbnailSize
        ? new Vector2(itemData.ThumbnailTex.width, itemData.ThumbnailTex.height) * thumbnailScale
        : size;

    public RectTransform Rect => canvasRect;

    private void OnValidate()
    {
        UpdateData();
    }

    private void OnEnable()
    {
        UpdateData();
    }

    private void Start()
    {
        if (Cursor.InstExists())
        {
            Cursor.Inst.AddCursorEventListener(this);
        }
        else
        {
            Debug.LogError($"Instance of {nameof(Cursor)} not found!");
        }
    }

    private void OnDisable()
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

    private void UpdateData()
    {
        if (!itemData)
        {
            Debug.Log($"No item data set for crafting item object {name}!");
            if(Application.isPlaying)
            {
                gameObject.name = "Item_MissingItemData";
            }

            thumbnailImage.texture = Resources.Load<Texture2D>("TX_Error_Sprite");

            return;
        }
    
        gameObject.name = "Item_" + itemData.ItemName;
        thumbnailImage.texture = itemData.ThumbnailTex;
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size.x);
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size.y);
    }

    private void OpenItem()
    {
        if(!WindowManager.InstExists())
        {
            Debug.LogError($"Instance of {nameof(WindowManager)} not found! Cannot open item window.");
            return;
        }

        WindowManager.Inst.CreateWindow(itemData.WindowContent);
        Destroy(this.gameObject);
    }

    public void OnCursorEvent(Cursor.CursorEvent e)
    {
        //PROTOTYPE: combine items by dragging thumbnails

        if(e == Cursor.CursorEvent.EnterElement)
        {
            isHovered = true;
        }
        else if(e == Cursor.CursorEvent.ExitElement)
        {
            isHovered = false;
        }

        if(isHovered && e == Cursor.CursorEvent.LeftClickUp)
        {
            var dragTarget = Cursor.Inst.CurrentDragTarget;
            if (dragTarget)
            {
                var draggedItem = dragTarget.GetComponentInParent<CraftingItemThumbnail>(); //TODO: parent/child/only on GameObject itself?
                if (draggedItem)
                {
                    //TODO: make a singleton CraftingManager if doing this kind of crafting
                    var crafter = FindFirstObjectByType<Crafter>();
                    if (crafter)
                    {
                        crafter.TryCraft(new List<CraftingItemData>() { itemData, draggedItem.itemData });
                    }
                    else
                    {
                        Debug.LogError($"No {nameof(Crafter)} found in scene!");
                    }
                }
            }
        }
        /*
        if(Cursor.Inst.IsCursorOverElement(this) && e == Cursor.CursorEvent.LeftClickDown)
        {
            OpenItem();
        }
        */
    }
}
