using Crafting;
using DG.Tweening;
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

    protected override void PlaceObject(DraggableObject obj)
    {
        base.PlaceObject(obj);

        var card = (CraftingItem)obj;
        if(!card)
        {
            return;
        }

        card.transform.SetParent(cardParent);
        hand.Add(card);
        SpreadCards();
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
            tForm.DOBlendableLocalMoveBy(pos - tForm.localPosition, 0.5f);
            tForm.DOBlendableLocalRotateBy(new Vector3(0f, 0f, cardRoll), 0.5f);
        }
    }

    [ContextMenu("Add Test Card")]
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

        throw new System.NotImplementedException();
    }

    protected override bool CanRemovePlacedObj(DraggableObject obj)
    {
        return hand.Contains(obj as CraftingItem);
    }

    public override void OnCursorEvent(Cursor.EventID e)
    {
        base.OnCursorEvent(e);
    }
}
