using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CraftingItem : MonoBehaviour
{
    [SerializeField] private CraftingItemData itemData;

    [Header("UI")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RawImage thumbnail;

    private void OnValidate()
    {
        UpdateData();
    }

    private void OnEnable()
    {
        UpdateData();
    }

    private void UpdateData()
    {
        if (!itemData)
        {
            Debug.Log($"No item data set for crafting item object {name}!");
            if(Application.isPlaying)
            {
                gameObject.name = "Item_MissingItemData";
            }

            thumbnail.texture = Resources.Load<Texture2D>("TX_Error_Sprite");

            return;
        }
    
        gameObject.name = "Item_" + itemData.ItemName;
        thumbnail.texture = itemData.Thumbnail;
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemData.Size.x);
        canvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemData.Size.y);
    }
}
