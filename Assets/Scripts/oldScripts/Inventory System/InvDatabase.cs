using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Database", menuName = "Inventory/Database", order = 1)]
public class InvDatabase : ScriptableObject
{
    [Serializable]
    public class Itm
    {
        public string id;
        public Sprite invSprite;
        public string description;
    }

    public List<Itm> invObjects;

    public Dictionary<string, Itm> GenerateDictionary() // generate a dictionary based on the list
    {
        Dictionary<string, Itm> invObjectsDict = new Dictionary<string, Itm>();

        for (int i = 0; i < invObjects.Count; i++)
        {
            invObjectsDict.Add(invObjects[i].id, invObjects[i]);
        }

        return invObjectsDict; // return generated dictionary
    }
}
