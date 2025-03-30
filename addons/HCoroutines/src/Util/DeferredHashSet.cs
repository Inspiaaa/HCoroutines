using System.Collections.Generic;

namespace HCoroutines.Util;

/// <summary>
/// HashSet implementation that defers Add() and Remove() calls during iteration.
/// </summary>
public class DeferredHashSet<T>
{
    public HashSet<T> Items = new();

    private bool isIterating = false;
    private HashSet<T> itemsToAdd = new();
    private HashSet<T> itemsToRemove = new();

    public void Add(T item)
    {
        if (isIterating)
        {
            itemsToAdd.Add(item);
            itemsToRemove.Remove(item);
        }
        else
        {
            Items.Add(item);
        }
    }

    public void Remove(T item)
    {
        if (isIterating)
        {
            itemsToRemove.Add(item);
            itemsToAdd.Remove(item);
        }
        else
        {
            Items.Remove(item);
        }
    }

    public void Lock()
    {
        isIterating = true;
    }

    public void Unlock()
    {
        isIterating = false;
        ExecutePendingAddRemoves();
    }

    private void ExecutePendingAddRemoves()
    {
        foreach (T item in itemsToAdd)
        {
            Items.Add(item);
        }
        itemsToAdd.Clear();

        foreach (T item in itemsToRemove)
        {
            Items.Remove(item);
        }
        itemsToRemove.Clear();
    }
}