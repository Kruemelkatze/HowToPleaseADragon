﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Shelf;
using UnityEditor;
using UnityEngine;

public class ShelfManager : SceneSingleton<ShelfManager>
{
    public ShelfDisplay Selected;

    public SpriteRenderer Selection;
    private Vector3 selectionPosition;

    private int SelectedIndex;

    public List<ShelfDisplay> Shelves;

    // Use this for initialization
    void Start()
    {
        SelectedIndex = 0;
        if (Shelves.Count > 0)
        {
            Selected = Shelves[SelectedIndex];
            Selection.transform.position = Selected.Frame.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int Add(int amount, ShelfDisplay shelf = null)
    {
        shelf = shelf != null ? shelf : Selected;
        
        var sum = shelf.CurrentAmount + amount;
        shelf.CurrentAmount = Math.Min(shelf.Shelf.TotalAmount, sum);

        if (sum < shelf.Shelf.TotalAmount)
            return 0;

        return sum - shelf.Shelf.TotalAmount;
    }

    public void Subtract(int amount)
    {
        Selected.CurrentAmount -= amount;

        if (Selected.CurrentAmount < 0)
        {
            Selected.CurrentAmount = 0;
        }
    }

    public void SelectLeft()
    {
        if (SelectedIndex > 0)
        {
            Selected = Shelves[--SelectedIndex];
            Selection.transform.position = Selected.Frame.transform.position;
        }
    }

    public void SelectRight()
    {
        if (SelectedIndex < Shelves.Count - 1)
        {
            Selected = Shelves[++SelectedIndex];
            Selection.transform.position = Selected.Frame.transform.position;
        }
    }

    public void UpgradeShelf(int index, int upgrade)
    {
        Shelf shelf = null;
        switch (upgrade)
        {
            case 0:
                shelf = Resources.Load<Shelf>("Shelf/BasicShelf");
                break;
            case 1:
                shelf = Resources.Load<Shelf>("Shelf/Upgrade1Shelf");
                break;
            case 2:
                shelf = Resources.Load<Shelf>("Shelf/Upgrade2Shelf");
                break;
            case 3:
                shelf = Resources.Load<Shelf>("Shelf/Upgrade3Shelf");
                break;
        }

        if (shelf != null)
        {
            Shelves[index].Shelf = shelf;
        }
    }
}