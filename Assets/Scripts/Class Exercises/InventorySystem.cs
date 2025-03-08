using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Dictionary<string,int> ItemNames = new Dictionary<string,int>();

        void AddItem(string name)
        {
            if (ItemNames.ContainsKey(name)){
                ItemNames[name]++;
            }
            else
            {
                ItemNames[name] = 1;
            }
        }

        void RemoveItem(string name)
        {
            if (ItemNames.ContainsKey(name) && ItemNames[name]>0)
            {
                ItemNames[name]--;
            }
            else if (ItemNames.ContainsKey(name) && ItemNames[name] == 1)
            {
                ItemNames.Remove(name);
            }
        }

        void Display(string name)
        {
            foreach (var item in ItemNames)
            {
                Debug.Log($"{item.Key}: {item.Value}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
