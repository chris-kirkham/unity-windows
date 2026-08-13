using Crafting;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Gallery : MonoBehaviour
{
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private Transform itemSpawnPoint;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI itemNameText; 
    [SerializeField] private TextMeshProUGUI itemTierText;
    [SerializeField] private TextMeshProUGUI itemPrerequisitesText;

    private CraftingItem currentItem;

    //TODO: replace prototype anims
    private async void SetItem(CraftingItemData itemData)
    {
        if(currentItem)
        {
            //TODO: Have prev/current/next item spawned always and slide them on and off?
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
        currentItem.transform.localScale = Vector3.zero;
        await currentItem.transform.DOScale(1f, 0.5f).AsyncWaitForCompletion();

        SetItemInfoUI(itemData);
    }

    private void SetItemInfoUI(CraftingItemData itemData)
    {
        itemNameText.text = itemData.ItemName;
        itemTierText.text = itemData.Tier.ToString();
        itemPrerequisitesText.text = itemData.Prerequisites.Count > 0 
            ? string.Join(", ", itemData.Prerequisites)
            : "None";
    }
}
