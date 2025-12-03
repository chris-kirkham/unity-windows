using UnityEngine;

[CreateAssetMenu(fileName = "CraftingItemData", menuName = "Crafting/ItemData")]
public class CraftingItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Texture2D thumbnail;
    [SerializeField] private WindowContent content;
    //TODO: PREREQUISITES/RECIPE FIELD
    [SerializeField] private Vector2 size = new Vector2(100f, 100f);
    [SerializeField] private bool useThumbnailSize = true;
    [SerializeField] private float thumbnailScale = 0.25f;

    public string ItemName => itemName;
    public Texture2D Thumbnail => thumbnail;
    public WindowContent Content => content;
    //public Vector2 DefaultSize => defaultSize;
    //public Vector2 Size => new Vector2(thumbnail.width, thumbnail.height) * scale;
    public Vector2 Size => useThumbnailSize
        ? new Vector2(thumbnail.width, thumbnail.height) * thumbnailScale
        : size;
}
