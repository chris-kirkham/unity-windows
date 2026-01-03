using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CraftingItemData", menuName = "Crafting/ItemData")]
public class CraftingItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private CraftingItemThumbnail thumbnail;
    [SerializeField] private Texture2D thumbnailTex;
    [SerializeField] private CraftingItemWindowContent contentPrefab;
    [SerializeField] private List<CraftingItemData> prerequisites;
    [SerializeField] private List<CraftingItemData> products;

    private HashSet<CraftingItemData> prerequisitesHash;

    public string ItemName => itemName;
    public CraftingItemThumbnail Thumbnail => thumbnail;
    public Texture2D ThumbnailTex => thumbnailTex;
    public CraftingItemWindowContent WindowContent => contentPrefab;
    public List<CraftingItemData> ExtraProducts => products;
    public List<CraftingItemData> Prerequisites => prerequisites;
        /*
    {
        get
        {
            if(prerequisitesHash == null)
            {
                prerequisitesHash = new HashSet<CraftingItemData>(prerequisites.Count);
                foreach(var prereq in prerequisites)
                {
                    prerequisitesHash.Add(prereq);
                }
            }

            return prerequisitesHash;
        }
    }
        */

    public override string ToString()
    {
        return itemName;
    }
}
