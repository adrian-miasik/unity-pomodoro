using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace AdrianMiasik.Components.Base
{
    /// <summary>
    /// A **generic** class that keeps track of our current (and previous) generic item selection in a collection.
    /// (e.g. radio button, selecting one element out of a list).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemSelector<T> : ThemeElement
    {
        [SerializeField] protected Collection<T> m_items = new Collection<T>();

        private T currentItem;
        private int currentIndex;
        
        private T lastSelectedItem;
        
        /// <summary>
        /// Invoked when the selection changes
        /// </summary>
        /// <param name="previousSelection"></param>
        /// <param name="currentSelection"></param>
        public delegate void OnSelectionChange(T previousSelection, T currentSelection);
        public OnSelectionChange onSelectionChange;
        
        /// <summary>
        /// Invoked when an item gets selected
        /// </summary>
        /// <param name="selectedItem"></param>
        public delegate void OnSelected(T selectedItem);
        public OnSelected onSelected;
        
        /// <summary>
        /// Invoked when an item gets deselected
        /// </summary>
        /// <param name="deselectedItem"></param>
        public delegate void OnDeselected(T deselectedItem);
        public OnDeselected onDeselected;

        /// <summary>
        /// Initializes with the serialized list
        /// </summary>
        private void Initialize()
        {
            currentIndex = 0;
            currentItem = m_items[currentIndex];
        }

        /// <summary>
        /// Initializes with a specific set of objects
        /// </summary>
        /// <param name="allSelectionItems"></param>
        public virtual void Initialize(PomodoroTimer pomodoroTimer, IEnumerable<T> allSelectionItems, bool updateColors = false)
        {
            base.Initialize(pomodoroTimer, updateColors);
            
            foreach (T item in allSelectionItems)
            {
                m_items.Add(item);
            }

            Initialize();
        }
        
        public void Clear()
        {
            m_items.Clear();
            currentItem = default;
        }
        
        public void AddItem(T item)
        {
            m_items.Add(item);
        }

        public void RemoveItem(T item)
        {
            m_items.Remove(item);
        }
        
        /// <summary>
        /// Selects the previous item in the collection
        /// </summary>
        public void PreviousItem()
        {
            ChangeIndex(-1);
            Select(currentIndex);
        }

        /// <summary>
        /// Selects the next item in the collection
        /// </summary>
        public void NextItem()
        {
            ChangeIndex(1);
            Select(currentIndex);
        }
        
        /// <summary>
        /// Increment/decrement our current index while staying within the bounds of our list by wrapping.
        /// </summary>
        /// <param name="difference"></param>
        private void ChangeIndex(int difference)
        {
            currentIndex += difference;
            currentIndex = (currentIndex + m_items.Count) % m_items.Count;
        }
        
        /// <summary>
        /// If your item is within our list, we will set the current item to it.
        /// </summary>
        /// <param name="itemInList"></param>
        public bool Select(T itemInList)
        {
            if (!m_items.Contains(itemInList))
            {
                Debug.LogWarning("Item not set! Unable to find the provided item.");
                return false;
            }

            ChangeSelection(itemInList);
            onSelectionChange?.Invoke(lastSelectedItem, currentItem);
            return true;
        }

        /// <summary>
        /// If your index is valid, we will set the current item to the object at the provided index and return true.
        /// Otherwise we will only return false. (No current item change)
        /// </summary>
        /// <param name="index"></param>
        private bool Select(int index)
        {
            if (!IsIndexValid(index))
            {
                Debug.LogWarning("Item not set! Invalid index.");
                return false;
            }

            ChangeSelection(m_items[index]);
            return true;
        }

        /// <summary>
        /// Returns true if the index is within the bounds of the elements list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsIndexValid(int index)
        {
            return index >= 0 && index < m_items.Count;
        }

        private void ChangeSelection(T itemInList)
        {
            // Deselect
            lastSelectedItem = currentItem;
            onDeselected?.Invoke(currentItem);

            // Swap
            currentItem = itemInList;
            currentIndex = m_items.IndexOf(itemInList);

            // Select
            onSelected?.Invoke(currentItem);
        }
        
        /// <summary>
        /// Returns the current visible item
        /// </summary>
        /// <returns></returns>
        public T GetCurrentItem()
        {
            return currentItem;
        }

        public int GetCurrentIndex()
        {
            return currentIndex;
        }

        public int GetCount()
        {
            return m_items.Count;
        }

        public T GetLastSelectedItem()
        {
            return lastSelectedItem;
        }

        public Collection<T> GetItems()
        {
            return m_items;
        }
    }
}