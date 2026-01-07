using System.Collections.Generic;
using UnityEngine;

public class CraftingInventory : MonoBehaviour
{
    [SerializeField] private Transform itemListRoot;
    
    private List<CraftingItem> items;
}
