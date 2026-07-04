using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Crafting;

public class ItemShowcaseSequence : MonoBehaviour
{
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private Transform itemSpawnInPos;
    [SerializeField] private Transform itemShowcasePos;
    [SerializeField] private Transform itemDespawnPos;
    [SerializeField] private float itemAnimateInTime = 1f;
    [SerializeField] private float itemShowcaseTime = 1f;
    [SerializeField] private float itemAnimateOutTime = 1f;

    private void Start()
    {
        StartCoroutine(CraftedItemShowcaseRoutine());
    }

    [ContextMenu("Test crafted item showcase")]
    private void TestShowcase()
    {
        gameObject.SetActive(true);
        StartCoroutine(CraftedItemShowcaseRoutine());
    }

    private IEnumerator CraftedItemShowcaseRoutine()
    {
        if(playableDirector)
        {
            playableDirector.Pause();
        }

        foreach(var itemData in craftingManager.GetUniqueItemsCrafted())
        {
            yield return ShowcaseItemRoutine(itemData);
        }

        if(playableDirector)
        {
            playableDirector.Resume();
        }
    
        gameObject.SetActive(false);
    }

    private IEnumerator ShowcaseItemRoutine(CraftingItemData itemData)
    {
        var item = craftingManager.SpawnItem(itemData, itemSpawnInPos.position, itemSpawnInPos.rotation);
        item.SetState(CraftingItem.State.Animatable);
        yield return Tweening.DoTransform(item.transform, itemShowcasePos.position, itemShowcasePos.rotation, item.transform.localScale, itemAnimateInTime);
        item.SetOnInspectVFX(true);
        yield return new WaitForSeconds(itemShowcaseTime);
        item.SetOnInspectVFX(false);
        yield return Tweening.DoTransform(item.transform, itemDespawnPos.position, itemDespawnPos.rotation, item.transform.localScale, itemAnimateOutTime);
        item.SetState(CraftingItem.State.Active);
        Destroy(item.gameObject);
    }
}
