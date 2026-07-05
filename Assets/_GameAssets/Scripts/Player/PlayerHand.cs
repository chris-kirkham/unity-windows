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

    private List<CraftingItem> cards = new List<CraftingItem>();

    protected override void PlaceObject(DraggableObject obj)
    {
        base.PlaceObject(obj);

        var card = (CraftingItem)obj;
        if(!card)
        {
            return;
        }

        card.transform.SetParent(cardParent);
        cards.Add(card);
        SpreadCards();
    }

    private void SpreadCards()
    {
        if(cards == null || cards.Count == 0)
        {
            return;
        }

        var leftmostPos = -cardOffset * (cards.Count / 2f);
        var cardRotation = Quaternion.Euler(0f, 0f, cardRoll);
        
        for(int i = 0; i < cards.Count; i++)
        {
            var pos = leftmostPos + (cardOffset * i);
            var tForm = cards[i].transform;
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

    protected override bool CanPlace(DraggableObject obj)
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
        throw new System.NotImplementedException();
    }

    public override void OnCursorEvent(Cursor.EventID e)
    {
        base.OnCursorEvent(e);


    }
}
