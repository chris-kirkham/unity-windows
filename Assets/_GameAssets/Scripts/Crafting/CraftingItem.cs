using System;
using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using FMODUnity;
using Crafting;
using Fusion;

[System.Serializable]
public class CraftingItem : DraggablePhysicsObject, ITargetable, IHaveHealth
{
    /// <summary>
    /// Animatable = crafting OFF, input OFF, kinematic rb, collision OFF
    /// Draggable = crafting OFF, input ON, kinematic rb, collision OFF
    /// Active = crafting ON, input ON, non-kinematic rb, collision ON
    /// </summary>
    public enum State
    {
        Animatable,
        Draggable,
        Active
    }

    [Header("Item data")]
    [SerializeField] private CardData itemData;

    [Header("Art")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private Renderer image;
    [SerializeField] private RectTransform imageArea;
    [SerializeField] private GameObject mirrorableArtRoot;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private FadeInOutText nameTextFade;
    [SerializeField] private TextMeshProUGUI debugImageText; //debug text for when image is missing
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("VFX")]
    [SerializeField] private GameObject onCraftedVFX;
    [SerializeField] private GameObject craftingPotentialVFX;

    [Header("SFX")]
    [SerializeField] private EventReference onGrabSFX;
    [SerializeField] private EventReference onDropSFX;

    private CraftingManager craftingManager;
    private Player owningPlayer;
    private int currHealth;
    private Material imageMat;
    private GameObject mirroredArtInstance;
    private bool acceptInput = true;
    private bool canBeUsedInCraft = true;
    private bool isTouchingOtherItems;
    private State state;

    public CardData Data
    {
        get => itemData;
        set
        {
            itemData = value;
            UpdateData();
        }
    }
    
    public event Action OnUsedInSuccessfulCraft;
    public event Action<int> OnHealthChange;

    protected override void OnDisable()
    {
        base.OnDisable();

        if (craftingManager)
        {
            craftingManager.OnItemDisabledOrDestroyed(this);
        }

        RemoveAllItemContacts();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBeUsedInCraft)
        {
            return;
        }

        var otherItem = other.GetComponentInParent<CraftingItem>();
        if (otherItem && otherItem.canBeUsedInCraft)
        {
            AddItemContact(otherItem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var otherItem = other.gameObject.GetComponentInParent<CraftingItem>();
        if (otherItem)
        {
            RemoveItemContact(otherItem);
        }
    }

    private void AddItemContact(CraftingItem item)
    {
        if (craftingManager)
        {
            craftingManager.AddItemContact(this, item);
        }
    }

    private void RemoveItemContact(CraftingItem item)
    {
        if (craftingManager)
        {
            craftingManager.RemoveItemContact(this, item);
        }

        if (!isTouchingOtherItems) //TODO: add this functionality back in
        {
            SetPartialCraftVFX(false);
        }
    }

    private void RemoveAllItemContacts()
    {
        if (craftingManager)
        {
            craftingManager.RemoveAllItemContactsForItem(this);
        }
    }

    private void UpdateData()
    {
        if (!itemData)
        {
            Debug.Log($"No item data set for crafting item object {name}!");
            if (Application.isPlaying)
            {
                gameObject.name = "Item_MissingItemData";
            }

            if (debugImageText)
            {
                debugImageText.text = "NO ITEM DATA";
                debugImageText.gameObject.SetActive(true);
            }

            return;
        }

        if (nameText)
        {
            nameText.text = itemData.ItemName;
        }

        gameObject.name = "Item_" + itemData.ItemName;

        if (craftingManager)
        {
            var itemsInPlay = craftingManager.ActiveItems;
            int sameItemCount = 0;
            foreach (var item in itemsInPlay)
            {
                if (item.itemData == itemData)
                {
                    sameItemCount++;
                }
            }

            if (sameItemCount > 0)
            {
                gameObject.name += $" ({sameItemCount})";
            }
        }

        if (image && itemData.ThumbnailTex && Application.isPlaying) //don't get material instance + default to text if not playing
        {
            imageMat = image.material;
            if (imageMat)
            {
                imageMat.mainTexture = itemData.ThumbnailTex;
                imageMat.SetTexture("_EmissionMap", itemData.ThumbnailTex);
                image.gameObject.SetActive(true);
            }

            if (debugImageText)
            {
                debugImageText.gameObject.SetActive(false);
            }
        }
        else if (debugImageText) //if no image set, use debug text
        {
            debugImageText.text = itemData.ItemName;
            debugImageText.gameObject.SetActive(true);
            image.gameObject.SetActive(false);
        }

        //mirror card art on rear
        /*
        if (mirrorableArtRoot)
        {
            if (mirroredArtInstance)
            {
                Destroy(mirroredArtInstance);
            }

            //TODO: Need to change to network runner spawn?
            mirroredArtInstance = Instantiate<GameObject>(
                mirrorableArtRoot,
                mirrorableArtRoot.transform.parent,
                worldPositionStays: true);
            mirroredArtInstance.transform.localScale = new Vector3(-1f, 1f, -1f);
        }
        */
    }

    [ContextMenu("Execute actions")]
    private async void ExecuteActions()
    {
        foreach(var action in Data.Actions)
        {
            if(!action)
            {
                Debug.LogError($"Null {nameof(CardAction)} in actions list for {Data.ItemName}!");
                continue;
            }

            action.Initialise(owningPlayer, this);
            await action.Execute();
        }
    }

    [ContextMenu("Cancel action execution")]
    public void CancelActions()
    {
        foreach(var action in Data.Actions)
        {
            action.Cancel();
        }
    }

    protected override void OnStartDrag()
    {
        SetCollisionEnabled(false);
        SetPartialCraftVFX(false);
        FMODX.PlayOneShotAttached(onGrabSFX, gameObject);
    }

    protected override void OnEndDrag()
    {
        base.OnEndDrag();

        SetCollisionEnabled(true);
        FMODX.PlayOneShotAttached(onDropSFX, gameObject);
    }

    //called when this item is first crafted
    //[Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true, Channel = RpcChannel.Reliable, TickAligned = true)]
    public void Initialise(Player owningPlayer, CraftingManager craftingManager, CardData itemData, Cursor cursor, bool wasCrafted)
    {
        this.owningPlayer = owningPlayer;

        if (craftingManager)
        {
            this.craftingManager = craftingManager;
            craftingManager.RegisterCraftingItem(this);
        }
        else
        {
            Debug.LogError($"Given {nameof(CraftingManager)} is null! This {nameof(CraftingItem)} will not be properly initialised.");
        }

        base.Initialise(cursor);
        
        Data = itemData;

        SetState(State.Active);

        if (craftingPotentialVFX)
        {
            craftingPotentialVFX.SetActive(false);
        }

        //TODO/PLACEHOLDER: Execute card actions on craft!
        if(wasCrafted)
        {
            ExecuteActions();
        }
    }

    public void OnCraftAttempt(CraftingManager.CraftingResultState resultState)
    {
        if (resultState == CraftingManager.CraftingResultState.SuccessfulCraft)
        {
            SetPartialCraftVFX(false);

            if (onCraftedVFX)
            {
                onCraftedVFX.SetActive(true);
            }

            SetCollisionEnabled(false);
            OnUsedInSuccessfulCraft?.Invoke();
            Destroy(gameObject);
        }
        else if (resultState == CraftingManager.CraftingResultState.PartialIngredientMatch)
        {
            //TODO: prototype partial craft VFX
            var seq = DOTween.Sequence(transform)
                .Append(transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true, ShakeRandomnessMode.Full));
                //.Join(transform.DOShakeRotation(0.5f));
            SetPartialCraftVFX(true);
        }
        else
        {
            SetPartialCraftVFX(false);
        }
    }

    public void SetPartialCraftVFX(bool on)
    {
        if (craftingPotentialVFX)
        {
            craftingPotentialVFX.SetActive(on);
        }
    }

    public void SetState(State state)
    {
        this.state = state;
        SetPhysAndCollision(state == State.Active);
        SetCanBeUsedInCraft(state == State.Active);
        SetAcceptInput(state == State.Draggable || state == State.Active);
    }

    private void SetCanBeUsedInCraft(bool enabled)
    {
        canBeUsedInCraft = enabled;

        if (!canBeUsedInCraft)
        {
            RemoveAllItemContacts();
        }
    }
    private void SetCollisionEnabled(bool enabled)
    {
        if (coll)
        {
            coll.enabled = enabled;

            if (!enabled)
            {
                RemoveAllItemContacts();
            }
        }
        else
        {
            Debug.LogError($"No Collider set to enable/disable collision on!");
        }
    }

    private void SetPhysAndCollision(bool enabled)
    {
        //rb.isKinematic = !enabled;
        rb.isKinematic = true;
        SetCollisionEnabled(enabled);
    }

    private void SetAcceptInput(bool acceptInput)
    {
        this.acceptInput = acceptInput;
        SetDragEnabled(acceptInput);

        if (!acceptInput)
        {
            RemoveAllItemContacts();
        }
    }

    public void SetOnInspectVFX(bool inspecting)
    {
        if (inspecting)
        {
            if (nameTextFade)
            {
                nameTextFade.FadeIn();
            }
        }
        else
        {
            if (nameTextFade)
            {
                nameTextFade.FadeOut();
            }
        }
    }

#region ITargetable
    public Vector3 GetTargetPosition()
    {
        return transform.position;
    }

    public Transform GetTargetTransform()
    {
        return transform;
    }

    public ITargetable.TargetableType GetTargetType()
    {
        return ITargetable.TargetableType.Card;
    }

    public void SetTargetingPreviewVisible(bool visible)
    {
        Debug.LogError("TODO: Targeting preview for cards");
    }
    #endregion

#region IHaveHealth
    public void SetHealth(int newHealth)
    {
        currHealth = newHealth;
        OnHealthChange?.Invoke(currHealth);
        if(currHealth <= 0)
        {
            OnKilled();
        }
    }

    public void DamageHealth(int damage)
    {
        SetHealth(currHealth - damage);
    }
#endregion

    private void OnKilled()
    {

    }
}
