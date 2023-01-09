using System;
using System.Collections.Generic;
using UnityEngine;

namespace LeTai.TrueShadow
{
[ExecuteAlways]
public class ShadowSorter : MonoBehaviour
{
#region SortDataContainer
    readonly struct SortEntry : IComparable<SortEntry>
    {
        public readonly TrueShadow shadow;
        public readonly Transform  shadowTransform;
        public readonly Transform  rendererTransform;

        public SortEntry(TrueShadow shadow)
        {
            this.shadow       = shadow;
            shadowTransform   = shadow.transform;
            rendererTransform = shadow.shadowRenderer.transform;
        }

        public int CompareTo(SortEntry other)
        {
            return other.shadowTransform.GetSiblingIndex().CompareTo(shadowTransform.GetSiblingIndex());
        }
    }

    readonly struct SortGroup
    {
        public readonly Transform       parentTransform;
        public readonly List<SortEntry> sortEntries;

        public SortGroup(SortEntry firstEntry)
        {
            sortEntries     = new List<SortEntry> {firstEntry};
            parentTransform = firstEntry.shadowTransform.parent;
        }

        public void Add(SortEntry pair)
        {
            if (pair.shadowTransform.parent != parentTransform)
                return;

            var index = sortEntries.BinarySearch(pair);
            if (index < 0)
                sortEntries.Insert(~index, pair);
        }

        public override int GetHashCode()
        {
            return parentTransform.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is SortGroup other && other.parentTransform == parentTransform;
        }
    }
#endregion


    private static ShadowSorter instance;

    public static ShadowSorter Instance
    {
        get
        {
            if (!instance)
            {
                var existings = FindObjectsOfType<ShadowSorter>();
                for (int i = existings.Length - 1; i > 0; i--)
                {
                    Destroy(existings[i]);
                }

#if UNITY_EDITOR
                var hidden = GameObject.Find("/" + nameof(ShadowSorter));
                while (hidden)
                {
                    DestroyImmediate(hidden);
                    hidden = GameObject.Find("/" + nameof(ShadowSorter));
                }
#endif

                instance = existings.Length > 0 ? existings[0] : null;

                if (!instance)
                {
                    var obj = new GameObject(nameof(ShadowSorter)) {
#if LETAI_TRUESHADOW_DEBUG
                        hideFlags = DebugSettings.Instance.showObjects
                                        ? HideFlags.DontSave
                                        : HideFlags.HideAndDontSave
#else
                        hideFlags = HideFlags.HideAndDontSave
#endif
                    };
                    instance = obj.AddComponent<ShadowSorter>();
                }
            }

            return instance;
        }
    }

    readonly IndexedSet<TrueShadow> shadows    = new IndexedSet<TrueShadow>();
    readonly IndexedSet<SortGroup>  sortGroups = new IndexedSet<SortGroup>();

    public void Register(TrueShadow shadow)
    {
        shadows.AddUnique(shadow);
    }

    public void UnRegister(TrueShadow shadow)
    {
        shadows.Remove(shadow);
    }

    void LateUpdate()
    {
        if (!this) return;

        for (var i = 0; i < shadows.Count; i++)
        {
            var shadow = shadows[i];

            if (!shadow || !shadow.isActiveAndEnabled)
                continue;

            shadow.CheckHierarchyDirtied();
            if (shadow.HierachyDirty)
                AddSortEntry(shadow);
        }

        Sort();
    }

    void AddSortEntry(TrueShadow shadow)
    {
        var entry    = new SortEntry(shadow);
        var group    = new SortGroup(entry);
        var oldIndex = sortGroups.IndexOf(group);
        if (oldIndex > -1)
            sortGroups[oldIndex].Add(entry);
        else
            sortGroups.Add(group);
    }

    public void Sort()
    {
        for (var i = 0; i < sortGroups.Count; i++)
        {
            var group = sortGroups[i];

            if (!group.parentTransform)
                continue;

            foreach (var entry in group.sortEntries)
            {
                entry.rendererTransform.SetParent(group.parentTransform, false);
                var rendererSid = entry.rendererTransform.GetSiblingIndex();
                var shadowSid   = entry.shadowTransform.GetSiblingIndex();
                if (rendererSid > shadowSid)
                {
                    entry.rendererTransform.SetSiblingIndex(shadowSid);
                }
                else
                {
                    entry.rendererTransform.SetSiblingIndex(shadowSid - 1);
                }

                entry.shadow.UnSetHierachyDirty();
            }

            // This is a separated loop, as siblind index of an entry will be affected by the laters
            foreach (var entry in group.sortEntries)
            {
                entry.shadow.ForgetSiblingIndexChanges();
            }
        }

        sortGroups.Clear();
    }

    void OnApplicationQuit()
    {
        // make sure object are recreated when exit play mode. Otherwise it turn into some weird state. need more research
        Destroy(gameObject);
    }
}
}
