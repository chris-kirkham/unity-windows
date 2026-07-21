using Crafting;
using DG.Tweening;
using Fusion;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class PlayerHand : DraggablePlacementPoint, ICursorEventListener, IAfterSpawned
{
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private int maxCards;
    [SerializeField] private Vector3 cardOffset;
    [SerializeField] private Vector3 selectedCardOffset;
    [SerializeField] private float cardRoll;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject placementPreviewVFX;

    private List<CraftingItem> hand = new List<CraftingItem>();
    private bool cardPositionsNeedRefresh;

    private int selectedCardIdx;

    //used to find cards in hand hovered by the cursor
    private const int MaxHoveredListeners = 100;
    private CraftingItem[] cursorHoveredItems = new CraftingItem[MaxHoveredListeners]; 

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Spawned()
    {
        TEST_AddTestHand();
        cursor.DraggablesMgr.DragTargetChanged += OnDragTargetChanged;
        SetPlacementPreviewActive(null, false);
    }

    public void AfterSpawned()
    {
        SpreadCards();
    }

    public override void FixedUpdateNetwork()
    {
        if(cardPositionsNeedRefresh)
        {
            SpreadCards();
        }
    }

    protected override void PlaceObject(DraggableObject obj)
    {
        base.PlaceObject(obj);

        var card = (CraftingItem)obj;
        if (!card)
        {
            return;
        }

        //card.SetState(CraftingItem.State.Animatable);
        card.transform.SetParent(cardParent, worldPositionStays: true);
        hand.Add(card);

        cardPositionsNeedRefresh = true;

        SetPlacementPreviewActive(obj, false);
    }

    private void SpreadCards()
    {
        if(hand == null || hand.Count == 0)
        {
            return;
        }

        var cardRotation = Quaternion.Euler(0f, 0f, cardRoll);

        for(int i = 0; i < hand.Count; i++)
        {
            var tForm = hand[i].transform;
            var pos = GetCardPlacementPos_LS(i, hand.Count);
            if(i == selectedCardIdx && !cursor.CurrentDragTarget) //offset hovered card if not dragging anything
            {
                pos += selectedCardOffset;
            }

            //tForm.localPosition = pos;
            tForm.DOLocalMove(pos - tForm.localPosition, 0.5f);
            tForm.DOLocalRotate(new Vector3(0f, 0f, cardRoll), 0.5f);
        }

        cardPositionsNeedRefresh = false;
    }

    private Vector3 GetCardPlacementPos_LS(int cardIdx, int handCount)
    {
        var leftmostPos = -cardOffset * (hand.Count / 2f);
        return leftmostPos + (cardOffset * cardIdx);
    }

    [ContextMenu("Add test hand")]
    private void TEST_AddTestHand()
    {
        foreach(var card in hand)
        {
            GameObject.Destroy(card.gameObject);
        }
        hand.Clear();

        for(int i = 0; i < maxCards; i++)
        {
            TEST_AddTestCard();
        }
    }

    [ContextMenu("Add test card")]
    private void TEST_AddTestCard()
    {
        var itemList = craftingManager.ItemDatabase.ItemList;
        var card = craftingManager.SpawnItem(itemList[Random.Range(0, itemList.Count)], Vector3.zero, Quaternion.identity, cardParent);
        PlaceObject(card);
    }

    private void SetPlacementPreviewActive(DraggableObject obj, bool active)
    {
        if (placementPreviewVFX)
        {
            placementPreviewVFX.SetActive(active);
        }

        if(obj)
        {
            /*
            if(active)
            {
                //lerp card between its drag position and hand position - TODO: PROTOTYPE/PLACEHOLDER!
                var placementPos_WS = transform.TransformPoint(GetCardPlacementPos_LS(hand.Count - 1, hand.Count));
                obj.SetDragPositionOverride(Vector3.Lerp(obj.transform.position, placementPos_WS, 0.5f));
            }
            else
            {
                obj.SetDragPositionOverride(null);
            }
            */
        }
    }

    protected override bool CanPlace(DraggableObject obj, PlacementSource _)
    {
        var item = (CraftingItem)obj;   
        
        if(!item)
        {
            return false;
        }

        Debug.LogError($"TODO: implement {nameof(PlayerHand)} CanPlace properly!");
        return true;
    }

    protected override bool CanRemovePlacedObj(DraggableObject obj)
    {
        return hand.Contains(obj as CraftingItem);
    }

    protected override void OnPlacedObjRemoved(DraggableObject obj)
    {
        hand.Remove((CraftingItem)obj);
        cardPositionsNeedRefresh = true;
    }

    protected override void OnDragTargetChanged()
    {
        base.OnDragTargetChanged();

        var dragTarget = cursor.CurrentDragTarget;
        if(dragTarget && hand.Contains((CraftingItem)dragTarget))
        {
            TryRemovePlacedObj(dragTarget);
        }
    }

    protected override void OnDraggableEnterPlacementArea(DraggableObject obj)
    {
        base.OnDraggableEnterPlacementArea(obj);

        //TODO: add placement source to enter/exit placement area callbacks!
        if (CanPlace(obj, PlacementSource.Default))
        {
            SetPlacementPreviewActive(obj, true);
        }
    }

    protected override void OnDraggableExitPlacementArea(DraggableObject obj)
    {
        base.OnDraggableExitPlacementArea(obj);

        SetPlacementPreviewActive(obj, false);
    }

    public override void OnCursorEvent(Cursor.EventID e)
    {
        base.OnCursorEvent(e);

        if(cursor.IsHovered(this)) //should be a big trigger covering the whole hand area
        {
            UpdateSelectedCard();
        }
    }

    private void UpdateSelectedCard()
    {
        //TODO: this feels inefficient... but at least there are no raycasts!
        var hoveredItemsCount = cursor.GetHoveredListenersOfType<CraftingItem>(cursorHoveredItems);
        var idx = -1;
        var bestScore = float.MaxValue;
        var cursorPos = new Vector3(cursor.ClampedPosition_SS.x, cursor.ClampedPosition_SS.y, 0f);
        for(int i = 0; i < hoveredItemsCount; i++)
        {
            var item = cursorHoveredItems[i];
            if (hand.Contains(item))
            {
                var score = cursor.DraggablesMgr.GetDraggableScore(item.transform);
                if(score < bestScore)
                {
                    idx = hand.IndexOf(item);
                    bestScore = score;
                }
            }
        }

        SetSelectedCardIdx(idx);
    }

    private void SetSelectedCardIdx(int idx)
    {
        selectedCardIdx = idx;
        cardPositionsNeedRefresh = true; //TODO: make this more efficient so we're not repositioning all cards every time!
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.white;

        //card spread preview
        for(int i = 0; i < maxCards; i++)
        {
            var pos = GetCardPlacementPos_LS(i, maxCards);
            Gizmos.DrawCube(pos, new Vector3(1f, 0.1f, 1.5f)); //approximate card size
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}
