using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crafting;

public class CraftingItemDeck : DraggablePlacementPoint, ICursorEventListener
{
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private CraftingItemDatabase startingDeck;
	[SerializeField] private float itemHeight = 0.1f;
    [SerializeField] private float itemZOffset = 0.05f;
    [SerializeField] private bool playerCanPlaceCards = true;
    [SerializeField] private bool populateOnEnable;
    [SerializeField] private bool shuffleOnPopulate;
    [SerializeField] private bool singleItemType = true;
    //allow any item type to be placed when the deck is empty, even if it's a single-item-type deck.
    //If it is and this is false, the deck's ItemType must be set in code in order to place an item on an empty deck
    [SerializeField] private bool allowAnyItemTypeWhenEmpty = false; 
    [SerializeField] private float animateItemToDeckTime = 0.4f;
    [Header("VFX")]
    [SerializeField] private FadeInOut onHoverPreviewVFX;

    private List<CraftingItem> deck = new List<CraftingItem>();

    public CardData ItemType { get; set; }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (populateOnEnable)
        {
            PopulateDeck(startingDeck, shuffleOnPopulate);
        }
    }

    public CraftingItem PeekTopItem()
    {
        return deck.Count > 0 ? deck[deck.Count - 1] : null;
    }

    public bool IsEmpty()
    {
        return deck.Count == 0;
    }

    private Vector3 GetTopDeckPos()
    {
        return transform.position
            + (deck.Count * itemHeight * Vector3.up)
            + (deck.Count * itemZOffset * Vector3.forward);
    }

    private Vector3 GetBottomDeckPos()
    {
        return transform.position;
    }

    private IEnumerator TweenItemToDeck(CraftingItem item)
    {
        //for infinite decks, items tweening to the deck should look like they're merging/being absorbed into the deck,
        //as infinite decks should only ever contain one item which represents that item type
        //TODO: WE NEED TO ACTUALLY NOT ADD MORE ITEMS TO INFINITE DECKS, THIS IS JUST A VFX HACK
        if(GameplaySettings.InfiniteDecks && deck.Count > 1)
        {
            //TODO: scaling cards to 0 and not scaling them back when dragging makes them invisible LOL
            //Fix infinite decks properly!
            yield return Tweening.DoTransform(
                item.transform, GetBottomDeckPos(), transform.rotation, Vector3.zero, animateItemToDeckTime).WaitForCompletion();

            var topItem = PeekTopItem();
            if (TryRemovePlacedObj(topItem))
            {
                Destroy(topItem.gameObject);
            }
        }
        else //non-infinite decks should visually stack items
        {
            yield return Tweening.DoTransform(
                item.transform, GetTopDeckPos(), transform.rotation, animateItemToDeckTime).WaitForCompletion();
        }
    }

    private void PopulateDeck(CraftingItemDatabase deckItems, bool shuffle)
    {
        ClearDeck();

        if (deckItems == null)
        {
            return;
        }

        //create a temporary deck list to add spawned items into; they can then be shuffled (or not) before
        //being added to the deck properly via TryPlaceObject (could shortcut past this, but safer to do it via the DraggablePlacementPoint
        //stuff in case more is added to that later)
        var tempDeck = new List<CraftingItem>(deckItems.ItemList.Count); 
        
        for (int i = 0; i < deckItems.ItemList.Count; i++)
        {
            var itemData = deckItems.ItemList[i];
            var item = craftingManager.SpawnItem(itemData, transform.position, Quaternion.identity);
            tempDeck.Add(item);
        }

        if(shuffle)
        {
            Shuffle();
        }

        foreach(var item in tempDeck)
        {
            if (!TryPlaceObject(item, PlacementSource.Script))
            {
                Debug.LogError($"Unable to place item when populating deck for some reason!");
            }
        }
    }

    private void ClearDeck()
    {
        foreach (var item in deck)
        {
            GameObject.Destroy(item.gameObject);
        }

        deck.Clear();
    }

    private void GrabTopDeckItem()
    {
        if (IsEmpty())
        {
            return;
        }

        var item = PeekTopItem();
        if (item)
        {
            item.SetState(CraftingItem.State.Draggable);
            item.RequestDrag();
        }
    }

    private void RemoveTopItem()
    {
        if (deck.Count == 0)
        {
            return;
        }

        var item = PeekTopItem();
        deck.RemoveAt(deck.Count - 1);

        if (!item)
        {
            Debug.LogError($"Removed a null item from the deck! Why did the deck contain a null item?");
            return;
        }

        var itemData = item.Data;
        item.transform.parent = null;
        item.SetState(CraftingItem.State.Active);

        //TODO: prototype - infinite deck - spawn new item to replace removed one
        if (GameplaySettings.InfiniteDecks && deck.Count < 1)
        {
            var newItem = craftingManager.SpawnItem(itemData, GetTopDeckPos(), Quaternion.identity);
            if(!TryPlaceObject(newItem, PlacementSource.Script))
            {
                Debug.LogError("Unable to replace item in deck for some reason!");
            }
        }
    }

    protected override bool CanPlace(DraggableObject obj, PlacementSource placementSource)
    {
        if(!(obj is CraftingItem))
        {
            return false;
        }

        if(!playerCanPlaceCards && placementSource == PlacementSource.PlayerDragAndDrop)
        {
            return false;
        }

        if(!singleItemType || (IsEmpty() && allowAnyItemTypeWhenEmpty))
        {
            return true;
        }
        else
        {
            return ((CraftingItem)obj).Data == ItemType;
        }
    }

    protected override void PlaceObject(DraggableObject obj)
    {
        if(obj is CraftingItem)
        {
            var item = (CraftingItem)obj;
            if (singleItemType)
            {
                ItemType = item.Data;
            }

            AddItemToTopDeck(item);
        }
        else
        {
            Debug.Log($"Tried to place a non-crafting item object on this deck! This should have been caught earlier.");
        }
    }

    private void AddItemToTopDeck(CraftingItem item, bool animateToDeck = true)
    {
        if(!item)
        {
            Debug.LogError($"Tried to add null item to deck!");
            return;
        }

        if (singleItemType && !IsEmpty() && item.Data != PeekTopItem().Data)
        {
            Debug.LogError("Added a different item type to a single-item deck! This should be dealt with earlier in code.");
        }

        if(GameplaySettings.InfiniteDecks && !IsEmpty())
        {
            //TODO: for infinite decks, still tween item to the deck but make it look like it's merging with the first item or something
            //infinite decks should basically look like one card which represents that item type - cards should be able to be placed/returned
            //to the deck but it shouldn't stack as if there are multiple (but finite) cards
            //maybe make cards added to infinite decks scale down to zero as they move to the deck?
        }

        item.transform.parent = transform;
        item.SetState(CraftingItem.State.Animatable);

        deck.Add(item);

        if (animateToDeck)
        {
            StartCoroutine(TweenItemToDeck(item));
        }
        else
        {
            item.transform.position = GetTopDeckPos();
        }
    }

    protected override bool CanRemovePlacedObj(DraggableObject obj)
    {
        return !IsEmpty() && PeekTopItem() == obj;
    }

    protected override void OnPlacedObjRemoved(DraggableObject obj)
    {
        if(PeekTopItem() == obj)
        {
            RemoveTopItem();
        }
        else
        {
            Debug.LogError($"Object {obj} removed from deck, but it isn't the top item in this deck! This shouldn't happen.");
        }
    }

    protected override void OnDraggableEnterPlacementArea(DraggableObject obj)
    {
        base.OnDraggableEnterPlacementArea(obj);

        if(CanPlace(obj, PlacementSource.PlayerDragAndDrop)) //TODO: will this always be player-placed?
        {
            SetPlacementPreviewVFXEnabled(true);
        }
    }

    protected override void OnDraggableExitPlacementArea(DraggableObject obj)
    {
        base.OnDraggableExitPlacementArea(obj);

        SetPlacementPreviewVFXEnabled(false);
    }

    protected override void OnDragTargetChanged()
    {
        base.OnDragTargetChanged();

        if (!cursor.CurrentDragTarget || !cursor.IsHovered(this))
        {
            SetPlacementPreviewVFXEnabled(false);
        }
    }

    private void SetPlacementPreviewVFXEnabled(bool enabled)
    {
        if (cursor && onHoverPreviewVFX)
        {
            onHoverPreviewVFX.gameObject.SetActive(enabled);
        }
    }

    [ContextMenu("Shuffle deck")]
    private void Shuffle()
    {
        if(deck == null || deck.Count < 2)
        {
            return;
        }
        
        //Fisher-Yates algorithm
        CraftingItem temp = null;
        Vector3 tempPos;
        for(int i = deck.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i);
            temp = deck[i];

            //swap item positions
            tempPos = deck[i].transform.position;
            deck[i].transform.position = deck[j].transform.position;
            deck[j].transform.position = tempPos;

            //swap items in list
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }

    

    //ICursorEventListener
    public override void OnCursorEvent(Cursor.EventID e)
    {
        base.OnCursorEvent(e);

        if (e == Cursor.EventID.LeftClickDown)
        {
            if (cursor.IsHovered(this))
            {
                GrabTopDeckItem();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        if (cursor && cursor.IsHovered(this))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 0f, 1f));
    }
}