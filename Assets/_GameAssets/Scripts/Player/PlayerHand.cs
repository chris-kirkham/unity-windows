using Crafting;
using DG.Tweening;
using Fusion;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHand : DraggablePlacementPoint
{
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private int maxCards;
    [SerializeField] private Vector3 cardOffset;
    [SerializeField] private float cardRoll;
    [SerializeField] private Transform cardParent;

    private List<CraftingItem> hand = new List<CraftingItem>();
    private bool cardPositionsNeedRefresh;

    public override void Spawned()
    {
        TEST_AddTestHand();
        cursor.DraggablesMgr.DragTargetChanged += OnDragTargetChanged;
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
        if(!card)
        {
            return;
        }

        card.transform.SetParent(cardParent, worldPositionStays: true);
        hand.Add(card);
        
        cardPositionsNeedRefresh = true;
    }

    private void SpreadCards()
    {
        if(hand == null || hand.Count == 0)
        {
            return;
        }

        var leftmostPos = -cardOffset * (hand.Count / 2f);
        var cardRotation = Quaternion.Euler(0f, 0f, cardRoll);

        for(int i = 0; i < hand.Count; i++)
        {
            var pos = leftmostPos + (cardOffset * i);
            var tForm = hand[i].transform;
            tForm.localPosition = pos;
            //tForm.DOBlendableLocalMoveBy(pos - tForm.localPosition, 0.5f);
            //tForm.DOBlendableLocalRotateBy(new Vector3(0f, 0f, cardRoll), 0.5f);
        }

        cardPositionsNeedRefresh = false;
    }

    [ContextMenu("Add test hand")]
    private void TEST_AddTestHand()
    {
        for(int i = 0; i < maxCards; i++)
        {
            TEST_AddTestCard();
        }
    }

    [ContextMenu("Add test card")]
    private void TEST_AddTestCard()
    {
        var itemList = craftingManager.ItemDatabase.ItemList;
        var card = craftingManager.SpawnItem(itemList[Random.Range(0, itemList.Count)], Vector3.zero, Quaternion.identity);
        PlaceObject(card);
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

    public override void OnCursorEvent(Cursor.EventID e)
    {
    }
}
