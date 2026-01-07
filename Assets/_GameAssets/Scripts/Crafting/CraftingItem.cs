using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class CraftingItem : MonoBehaviour, ICursorEventListener
{
    [SerializeField] private CraftingItemData itemData;
    
    [Header("UI")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RawImage thumbnailImage;
    [SerializeField] private Vector2Int itemSize = new Vector2Int(100, 100);
    [SerializeField] private RectTransform imageArea;

    [Header("VFX")]
    [SerializeField] private GameObject onCraftedVFX;

    private bool acceptInput = true;
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

    public RectTransform Rect => canvasRect;

    private void OnValidate()
    {
        UpdateData();
    }

    private void OnEnable()
    {
        SetAcceptInput(true);
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
        
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize.x);
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize.y);

        //scale thumbnail image so its longest side matches canvas size, maintaining aspect ratio
        var imageAreaWidth = imageArea.rect.width;
        var imageAreaHeight = imageArea.rect.height;
        var imageWidth = thumbnailImage.texture.width;
        var imageHeight = thumbnailImage.texture.height;
        var imageRect = thumbnailImage.rectTransform;
        //if (imageWidth > imageHeight) //fit entire image in
        if(imageHeight > imageWidth) //scale image up so smallest side fills image area (needs mask)
        {
            var scale = imageAreaWidth / (float)imageWidth;
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageAreaWidth);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageHeight * scale);
        }
        else 
        {
            var scale = imageAreaHeight / (float)imageHeight;
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageWidth * scale);
            imageRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageAreaHeight);
        }
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

    //accept or block user input (e.g. for when animating item)
    private void SetAcceptInput(bool acceptInput)
    {
        this.acceptInput = acceptInput;
        if(!acceptInput) //necessary?
        {
            isHovered = false;
        }
    }

    public void OnCursorEvent(Cursor.CursorEvent e)
    {
        if(!acceptInput)
        {
            return;
        }
        
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
                var draggedItem = dragTarget.GetComponentInParent<CraftingItem>(); //TODO: parent/child/only on GameObject itself?
                if (draggedItem)
                {
                    //TODO: make a singleton CraftingManager if doing this kind of crafting
                    var crafter = FindFirstObjectByType<Crafter>();
                    if (crafter)
                    {
                        crafter.TryCraft(new List<CraftingItem>() { this, draggedItem }); //TODO: refactor
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

    //called when this item is first crafted
    public void OnCrafted()
    {
        //TODO: other VFX
        OnCraftedVFX();
        StartCoroutine(OnCraftedAnim(Rect, Cursor.Inst.ClampedPosition_WS));
    }

    public void OnUsedInCraft(bool successful)
    {
        //TODO: behaviour on successful/unsuccessful craft attempt
        if(successful)
        {
            StartCoroutine(OnSuccessfulCraftAnim());
        }
        else
        {
            //TODO?
        }
    }

    private IEnumerator OnSuccessfulCraftAnim()
    {
        //TODO: other VFX
        SetAcceptInput(false);
        yield return OnCraftedAnim(Rect, Cursor.Inst.ClampedPosition_WS);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    //TODO: prototype/placeholder
    private IEnumerator OnCraftedAnim(RectTransform item, Vector2 mousePos)
    {
        var startPos = item.position;
        var targetPos = mousePos + (Vector2)Random.onUnitSphere * Random.Range(200f, 400f);

        var startRot = item.rotation;
        var targetRot = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f));

        var time = Random.Range(0.5f, 0.75f);
        var t = 0f;
        while (t < 1f)
        {
            item.position = Vector3.Lerp(startPos, targetPos, t);
            item.rotation = Quaternion.Lerp(startRot, targetRot, t);
            t += Time.deltaTime / time;
            yield return null;
        }
    }

    private void OnCraftedVFX()
    {
        if(onCraftedVFX)
        {
            onCraftedVFX.SetActive(true);
        }
    }
}
