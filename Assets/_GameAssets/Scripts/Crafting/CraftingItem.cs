using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CraftingItem : MonoBehaviour, ICursorEventListener
{
    [SerializeField] private CraftingItemData itemData;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider coll;
    [SerializeField] private float onCraftedCollisionEnableDelay = 0.5f;

    [Header("UI")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RawImage thumbnailImage;
    [SerializeField] private Vector2Int itemSize = new Vector2Int(100, 100);
    [SerializeField] private RectTransform imageArea;
    [SerializeField] private TextMeshProUGUI debugImageText; //debug text for when image is missing

    [Header("VFX")]
    [SerializeField] private GameObject onCraftedVFX;
    [SerializeField] private GameObject craftingPotentialVFX;

    private bool acceptInput = true;
    private bool isHovered;

    private HashSet<CraftingItem> touchingItems;

    public HashSet<CraftingItem> TouchingItems => touchingItems;

    public CraftingItemData Data 
    {
        get => itemData;
        set
        {
            itemData = value;
            UpdateData();
        } 
    } 

    private void OnValidate()
    {
        UpdateData();
    }

    private void OnEnable()
    {
        SetAcceptInput(true);
        UpdateData();

        touchingItems = new HashSet<CraftingItem>();
        touchingItems.Add(this); //an item is always touching itself

        if(craftingPotentialVFX)
        {
            //TODO: check for crafting potential on enable?
            craftingPotentialVFX.SetActive(false);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        var otherItem = other.GetComponentInParent<CraftingItem>();
        if(otherItem && !touchingItems.Contains(otherItem))
        {
            touchingItems.Add(otherItem);
            OnNewItemContact(otherItem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var otherItem = other.gameObject.GetComponentInParent<CraftingItem>();
        if (otherItem && touchingItems.Contains(otherItem))
        {
            touchingItems.Remove(otherItem);
            OnLostItemContact(otherItem);
        }
    }

    private void OnNewItemContact(CraftingItem item)
    {
        Crafter.Inst.AddItemContact(this, item);
    }

    private void OnLostItemContact(CraftingItem item)
    {
        Crafter.Inst.RemoveItemContact(this, item);
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

        if(itemData.ThumbnailTex)
        {
            thumbnailImage.gameObject.SetActive(true);
            thumbnailImage.texture = itemData.ThumbnailTex;
            if(debugImageText)
            {
                debugImageText.gameObject.SetActive(false);
            }
        }
        else if(debugImageText) //if no image set, use debug text
        {
            debugImageText.text = itemData.ItemName;
            debugImageText.gameObject.SetActive(true);
            thumbnailImage.gameObject.SetActive(false);
        }

        if(canvasRect)
        {
            canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemSize.x);
            canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemSize.y);
        }

        if (thumbnailImage.texture)
        {
            //scale thumbnail image so its longest side matches canvas size, maintaining aspect ratio
            var imageAreaWidth = imageArea.rect.width;
            var imageAreaHeight = imageArea.rect.height;
            var imageWidth = thumbnailImage.texture.width;
            var imageHeight = thumbnailImage.texture.height;
            var imageRect = thumbnailImage.rectTransform;
            //if (imageWidth > imageHeight) //fit entire image in
            if (imageHeight > imageWidth) //scale image up so smallest side fills image area (needs mask)
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
        
        //combine items by dragging them together. TODO: THIS IS ALL PROTOTYPE AND WILL NEED REFACTORING!
        if(e == Cursor.CursorEvent.EnterElement)
        {
            isHovered = true;
        }
        else if(e == Cursor.CursorEvent.ExitElement)
        {
            isHovered = false;
        }

        /*
        //TODO: refactor
        if (isHovered && e == Cursor.CursorEvent.LeftClickUp)
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
                        //get hovered items
                        var hoveredListeners = Cursor.Inst.HoveredListeners;
                        var hoveredCraftItems = new List<CraftingItem>(hoveredListeners.Count);
                        foreach(var listener in hoveredListeners)
                        {
                            if(listener is CraftingItem)
                            {
                                hoveredCraftItems.Add((CraftingItem)listener);
                            }
                        }

                        crafter.TryCraft(hoveredCraftItems); 
                        //crafter.TryCraft(new List<CraftingItem>() { this, draggedItem }); 
                    }
                    else
                    {
                        Debug.LogError($"No {nameof(Crafter)} found in scene!");
                    }
                }
            }
        }
        */

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
        //StartCoroutine(OnCraftedAnim(transform, Cursor.Inst.ClampedPosition_WS));

        SetCollisionEnabled(false);
        StartCoroutine(SetCollisionEnabledWithDelay(true, onCraftedCollisionEnableDelay));
    }

    private void OnCraftedVFX()
    {
        if (onCraftedVFX)
        {
            onCraftedVFX.SetActive(true);
        }
    }

    //TODO: prototype/placeholder
    private void OnCraftedPush()
    {
        var pushForce = Random.insideUnitCircle * 10f;
        rb.AddForce(new Vector3(pushForce.x, 0f, pushForce.y), ForceMode.VelocityChange);
        var pushTorque = new Vector3(0f, 0f, Random.Range(-15f, 15f));
        rb.AddTorque(pushTorque, ForceMode.VelocityChange);
    }

    public void OnCraftAttempt(Crafter.CraftingResultState resultState)
    {
        if(resultState == Crafter.CraftingResultState.SuccessfulCraft)
        {
            OnSuccessfulCraft();
        }
        else if(resultState == Crafter.CraftingResultState.PartialIngredientMatch)
        {
            SetPartialCraftVFX(true);
        }
        else
        {
            SetPartialCraftVFX(false);
        }
    }

    public void SetPartialCraftVFX(bool on)
    {
        craftingPotentialVFX.SetActive(on);
    }

    private void OnSuccessfulCraft()
    {
        SetCollisionEnabled(false);
        StartCoroutine(OnSuccessfulCraftAnim());
    }

    private IEnumerator OnSuccessfulCraftAnim()
    {
        //TODO: other VFX
        SetAcceptInput(false);
        OnCraftedPush();
        yield return new WaitForSeconds(1f);
        SetAcceptInput(true); //ONLY IF NOT DESTROYING ON CRAFT
        Destroy(gameObject);
    }

    private void SetCollisionEnabled(bool enabled)
    {
        if(coll)
        {
            coll.enabled = enabled;
        }
        else
        {
            Debug.LogError($"No Collider set to enable/disable collision on!");
        }
    }

    private IEnumerator SetCollisionEnabledWithDelay(bool enabled, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetCollisionEnabled(enabled);
    }
}
