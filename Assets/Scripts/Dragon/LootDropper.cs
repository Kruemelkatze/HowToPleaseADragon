﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootDropper : SceneSingleton<LootDropper>
{
    public GameObject Dropping;
    public int DropAmount = 200;

    public int ScaleBaseValue = 200;

    public bool ScaleWithValue = true;

    public TextAsset ItemsFile;
    public ItemDefinition[] ItemDefinitions;

    public float LootDropPercentage = 50;


    void Awake()
    {
        SetInstance();
        LoadItems();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LoadItems()
    {
        if (ItemsFile != null)
        {
            ItemDefinitions = JsonUtility.FromJson<ItemFileContent>(ItemsFile.text).Items;

            foreach (var itemDefinition in ItemDefinitions)
            {
                itemDefinition.ItemType = (ItemType) Enum.Parse(typeof(ItemType), itemDefinition.Type);
            }
        }
    }

    public void Drop()
    {
        Drop(DropAmount);
    }

    public void Drop(int amount)
    {
        var drop = GameObject.Instantiate(Dropping, transform);
        var lootDropScript = drop.GetComponent<LootDrop>();
        lootDropScript.Amount = amount;

        if (ScaleWithValue)
        {
            var scale = (float) DropAmount / ScaleBaseValue;
            drop.transform.localScale = new Vector3(scale, scale, scale);
        }
        
        if (Random.Range(0f, 100f) <= LootDropPercentage)
        {
            lootDropScript.Loot = GetRandomItem();
            Debug.Log("Dropped loot!");
            Debug.Log(JsonUtility.ToJson(lootDropScript.Loot));
        }

        lootDropScript.Init();
    }

    public ItemDefinition GetRandomItem()
    {

        double ShelfStatus = ShelfManager.Instance.LevelShelfesFull();
        Debug.Log("ShelfStatus" + ShelfStatus);
                
        // If shelves become too full, increase chance to get shelf upgrade items
        int index = GetRandomWeightedIndex(ItemDefinitions.Select(x => (x.Type == "ShelfUpgrade" && ShelfStatus > 0.8) ? x.Chance * 2 : x.Chance).ToArray());
        return ItemDefinitions[index];
    }

    // A courtesy of https://forum.unity.com/threads/random-numbers-with-a-weighted-chance.442190/
    private int GetRandomWeightedIndex(int[] weights)
    {
        // Get the total sum of all the weights.
        int weightSum = 0;
        for (int i = 0; i < weights.Length; ++i)
        {
            weightSum += weights[i];
        }

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Length - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }

            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }

        // No other item was selected, so return very last index.
        return index;
    }
}

[System.Serializable]
class ItemFileContent
{
    public ItemDefinition[] Items;
}