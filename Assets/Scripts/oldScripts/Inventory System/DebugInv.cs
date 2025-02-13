using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInv : MonoBehaviour
{
    [SerializeField] string itemName;

    [ContextMenu("Add Item")]
    public void Add()
    {
        InventoryStorage.instance.AddItem(itemName);
    }
}
