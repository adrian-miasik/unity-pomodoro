using System;
using System.Collections;
using System.Collections.Generic;

namespace LeTai.TrueShadow
{
class IndexedSet<T> : IList<T>
{
    readonly List<T>            list = new List<T>();
    readonly Dictionary<T, int> dict = new Dictionary<T, int>();

    public void Add(T item)
    {
        dict.Add(item, list.Count);
        list.Add(item);
    }

    public bool AddUnique(T item)
    {
        if (dict.ContainsKey(item))
            return false;

        dict.Add(item, list.Count);
        list.Add(item);

        return true;
    }

    public bool Remove(T item)
    {
        if (!dict.TryGetValue(item, out var index))
            return false;

        RemoveAt(index);
        return true;
    }

    public void Remove(Predicate<T> match)
    {
        int i = 0;
        while (i < list.Count)
        {
            T item = list[i];
            if (match(item))
                Remove(item);
            else
                i++;
        }
    }

    public void Clear()
    {
        list.Clear();
        dict.Clear();
    }

    public bool Contains(T item)
    {
        return dict.ContainsKey(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    public int Count => list.Count;

    public bool IsReadOnly => false;

    public int IndexOf(T item)
    {
        if (dict.TryGetValue(item, out var index))
            return index;
        return -1;
    }

    public void Insert(int index, T item)
    {
        //We could support this, but the semantics would be weird. Order is not guaranteed..
        throw new NotSupportedException(
            "Random Insertion is semantically invalid, since this structure does not guarantee ordering.");
    }

    public void RemoveAt(int index)
    {
        T item = list[index];
        dict.Remove(item);
        if (index == list.Count - 1)
            list.RemoveAt(index);
        else
        {
            int replaceItemIndex = list.Count - 1;
            T   replaceItem      = list[replaceItemIndex];
            list[index]       = replaceItem;
            dict[replaceItem] = index;
            list.RemoveAt(replaceItemIndex);
        }
    }

    public T this[int index]
    {
        get => list[index];
        set
        {
            T item = list[index];
            dict.Remove(item);
            list[index] = value;
            dict.Add(item, index);
        }
    }

    //Sorts the internal list, this makes the exposed index accessor sorted as well.
    //But note that any insertion or deletion, can unorder the collection again.
    public void Sort(Comparison<T> sortLayoutFunction)
    {
        //There might be better ways to sort and keep the dictionary index up to date.
        list.Sort(sortLayoutFunction);
        //Rebuild the dictionary index.
        for (int i = 0; i < list.Count; ++i)
        {
            T item = list[i];
            dict[item] = i;
        }
    }


    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
}
