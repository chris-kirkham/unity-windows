using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CraftingItemData", menuName = "Crafting/ItemData")]
public class CardData : ScriptableObject
{
    [SerializeField] private string itemName;
    //items with a crafting alias can be treated as that item during crafting (I'm sure this won't break anything)
    [SerializeField] private List<CardData> craftingAliases; 
    [SerializeField] private Texture2D thumbnailTex;
    [SerializeField] private CraftingItemWindowContent contentPrefab;
    [SerializeField] private List<CardData> prerequisites;
    [SerializeField] private List<CardData> products;
    [Header("Card Actions")]
    [SerializeField] private List<CardAction> actions;

    public int CraftingTier { get; set; }

    public bool HasPrerequisiteLoop { get; private set; }

    public string ItemName => itemName;
    public List<CardData> Aliases => craftingAliases;
    public Texture2D ThumbnailTex => thumbnailTex;
    public CraftingItemWindowContent WindowContent => contentPrefab;
    public List<CardData> ExtraProducts => products;
    public List<CardData> Prerequisites => prerequisites;

    public override string ToString()
    {
        return itemName;
    }

    private void OnValidate()
    {
        HasPrerequisiteLoop = CheckForPrerequisiteLoops();
    }

    //Get crafting prerequisites in the form {prerequisite, number required}
    //TODO: inefficient to calculate each time - maybe this should be the format the crafting manager
    //uses anyway, in which case it should be cached or the inspector changed so prerequisites can be defined
    //in this format to begin with
    public Dictionary<CardData, int> GetPrereqCounts()
    {
        var counts = new Dictionary<CardData, int>();
        foreach(var prereq in prerequisites)
        {
            if(counts.TryGetValue(prereq, out var count))
            {
                counts[prereq] = count + 1;
            }
            else
            {
                counts.Add(prereq, 1);
            }
        }

        return counts;
    }

    public bool CheckForPrerequisiteLoops()
    {
        var visited = new HashSet<CardData>(prerequisites.Count);
        var recursion = new HashSet<CardData>(prerequisites.Count);

        return NodeHasLoop(this);

        bool NodeHasLoop(CardData itemData)
        {
            if(!itemData)
            {
                return false;
            }

            //node already visited - loop detected!
            if (recursion.Contains(itemData))
            {
                Debug.LogError($"Prerequisite loop detected: {string.Join("->", recursion)}->{itemData}");
                return true;
            }

            //node already visited and no loop found
            if (visited.Contains(itemData))
            {
                return false;
            }

            //add this item to visited and recursion sets
            visited.Add(itemData);
            recursion.Add(itemData);

            foreach (var prereq in itemData.Prerequisites)
            {
                if (NodeHasLoop(prereq))
                {
                    return true;
                }
            }

            //remove this item from recursion stack if no loop found
            recursion.Remove(itemData);
            return false;
        }
    }
}
