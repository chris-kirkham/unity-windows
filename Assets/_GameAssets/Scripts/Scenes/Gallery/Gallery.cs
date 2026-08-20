using Crafting;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    [SerializeField] private CraftingManager craftingManager; //TODO: make sure this is the player's crafting manager
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private CraftingItemDatabase itemDatabase; //TODO: replace with saved player crafting progress data when you make it

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI itemNameText; 
    [SerializeField] private TextMeshProUGUI itemTierText;
    [SerializeField] private TextMeshProUGUI itemPrerequisitesText;
    [SerializeField] private TextMeshProUGUI itemPrerequisiteForText;
    [SerializeField] private SceneTransitionButton mainMenuButton;

    private CraftingItem currentItem;
    private int currentItemIndex;

    private List<CraftingItemData> craftedItems;

    private void OnEnable()
    {
        if(itemDatabase)
        {
            SetItemByIndex(0);
        }

        if(craftingManager)
        {
            craftedItems = craftingManager.GetUniqueItemsCrafted();
        }
    }

    //TODO: replace prototype anims
    private async void SetItemByIndex(int itemIdx)
    {
        currentItemIndex = itemIdx;
        var itemData = craftedItems[itemIdx];
        SetItemInfoUI(itemData);

        if(currentItem)
        {
            //TODO: Have prev/current/next item spawned always and slide them on and off?
            currentItem.SetFloatingVFX(false);
            await currentItem.transform.DOScale(0f, 0.5f).AsyncWaitForCompletion();
            GameObject.Destroy(currentItem.gameObject);
        }

        if(!itemData)
        {
            Debug.LogError($"Set {nameof(CraftingItemData)} is null!");
            return;
        }

        currentItem = craftingManager.SpawnItem(itemData, itemSpawnPoint.position, itemSpawnPoint.rotation, doItemOnCraftedCallback: false);
        currentItem.SetState(CraftingItem.State.Animatable);
        currentItem.SetFloatingVFX(true);
        currentItem.transform.localScale = Vector3.zero;
        await currentItem.transform.DOScale(1f, 0.5f).AsyncWaitForCompletion();
    }

    [ContextMenu("Prev item")]
    public void PrevItem()
    {
        currentItemIndex = currentItemIndex == 0 ? itemDatabase.ItemList.Count - 1 : currentItemIndex - 1;
        SetItemByIndex(currentItemIndex);
    }

    [ContextMenu("Next item")]
    public void NextItem()
    {
        currentItemIndex = (currentItemIndex + 1) % itemDatabase.ItemList.Count;
        SetItemByIndex(currentItemIndex);
    }

    private void SetItemInfoUI(CraftingItemData itemData)
    {
        itemNameText.text = itemData.ItemName;
        itemTierText.text = "Tier " + itemData.Tier.ToString();
        itemPrerequisitesText.text = itemData.Prerequisites.Count > 0
            ? string.Join(", ", itemData.Prerequisites)
            : "None";
        var prerequisitesFor = itemDatabase.GetItemPrerequisiteForList(itemData);
        itemPrerequisiteForText.text = prerequisitesFor.Count > 0 ? string.Join(", ", prerequisitesFor) : "None";
    }
}
