using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LeTai.TrueShadow
{
interface IChangeTracker
{
    void Check();
}

class ChangeTracker<T> : IChangeTracker
{
    T                         previousValue;
    readonly Func<T>          getValue;
    readonly Func<T, T>       onChange;
    readonly Func<T, T, bool> compare;

    public ChangeTracker(Func<T>          getValue,
                         Func<T, T>       onChange,
                         Func<T, T, bool> compare = null)
    {
        this.getValue = getValue;
        this.onChange = onChange;
        this.compare  = compare ?? EqualityComparer<T>.Default.Equals;

        previousValue = this.getValue();
    }

    public void Forget()
    {
        previousValue = getValue();
    }

    public void Check()
    {
        T newValue = getValue();
        if (!compare(previousValue, newValue))
        {
            previousValue = onChange(newValue);
        }
    }
}

public partial class TrueShadow
{
    Action               checkHierarchyDirtiedDelegate;
    IChangeTracker[]     transformTrackers;
    ChangeTracker<int>[] hierachyTrackers;

    void InitInvalidator()
    {
        checkHierarchyDirtiedDelegate = CheckHierarchyDirtied;
        hierachyTrackers = new[] {
            new ChangeTracker<int>(
                () => RectTransform.GetSiblingIndex(),
                newValue =>
                {
                    SetHierachyDirty();
                    return newValue; // + 1;
                }
            ),
            new ChangeTracker<int>(
                () =>
                {
                    if (shadowRenderer)
                        return shadowRenderer.transform.GetSiblingIndex();
                    return -1;
                },
                newValue =>
                {
                    SetHierachyDirty();
                    return newValue; // + 1;
                }
            )
        };

        transformTrackers = new IChangeTracker[] {
            new ChangeTracker<Vector3>(
                () => RectTransform.position,
                newValue =>
                {
                    SetLayoutDirty();
                    return newValue;
                },
                (prev, curr) => prev == curr
            ),
            new ChangeTracker<Quaternion>(
                () => RectTransform.rotation,
                newValue =>
                {
                    SetLayoutDirty();
                    if (Cutout)
                        SetTextureDirty();
                    return newValue;
                },
                (prev, curr) => prev == curr
            ),
        };

#if TMP_PRESENT
        if (Graphic is TMPro.TextMeshProUGUI)
        {
            var old = transformTrackers;
            transformTrackers = new IChangeTracker[old.Length + 1];
            Array.Copy(old, transformTrackers, old.Length);

            transformTrackers[old.Length] = new ChangeTracker<Vector3>(
                () => RectTransform.lossyScale,
                newValue =>
                {
                    SetLayoutTextureDirty();
                    return newValue;
                },
                (prev, curr) =>
                {
                    if (prev == curr) // Early exit for most common path
                        return true;

                    if (prev.x * prev.y * prev.z < 1e-9f
                     && curr.x * curr.y * curr.z > 1e-9f)
                        return false;

                    var diff = curr - prev;
                    return Mathf.Abs(diff.x / prev.x) < .25f
                        && Mathf.Abs(diff.y / prev.y) < .25f
                        && Mathf.Abs(diff.z / prev.z) < .25f;
                }
            );
        }
#endif

        Graphic.RegisterDirtyLayoutCallback(SetLayoutTextureDirty);
        Graphic.RegisterDirtyVerticesCallback(SetLayoutTextureDirty);
        Graphic.RegisterDirtyMaterialCallback(OnGraphicMaterialDirty);

        CheckHierarchyDirtied();
        CheckTransformDirtied();
    }

    void TerminateInvalidator()
    {
        if (Graphic)
        {
            Graphic.UnregisterDirtyLayoutCallback(SetLayoutTextureDirty);
            Graphic.UnregisterDirtyVerticesCallback(SetLayoutTextureDirty);
            Graphic.UnregisterDirtyMaterialCallback(OnGraphicMaterialDirty);
        }
    }

    void OnGraphicMaterialDirty()
    {
        SetLayoutTextureDirty();
        shadowRenderer.UpdateMaterial();
    }

    internal void CheckTransformDirtied()
    {
        if (transformTrackers != null)
        {
            for (var i = 0; i < transformTrackers.Length; i++)
            {
                transformTrackers[i].Check();
            }
        }
    }

    internal void CheckHierarchyDirtied()
    {
        if (ShadowAsSibling && hierachyTrackers != null)
        {
            for (var i = 0; i < hierachyTrackers.Length; i++)
            {
                hierachyTrackers[i].Check();
            }
        }
    }

    internal void ForgetSiblingIndexChanges()
    {
        for (var i = 0; i < hierachyTrackers.Length; i++)
        {
            hierachyTrackers[i].Forget();
        }
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        ApplySerializedData();

        if (ProjectSettings.Instance.UseGlobalAngleByDefault)
        {
            UseGlobalAngle = true;
        }
    }
#endif

    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();

        if (!isActiveAndEnabled) return;

        SetHierachyDirty();
        this.NextFrames(checkHierarchyDirtiedDelegate);
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        if (!isActiveAndEnabled) return;

        SetLayoutTextureDirty();
    }


    protected override void OnDidApplyAnimationProperties()
    {
        if (!isActiveAndEnabled) return;

        SetLayoutTextureDirty();
    }

    public void ModifyMesh(Mesh mesh)
    {
        if (!isActiveAndEnabled) return;

        if (SpriteMesh) Utility.SafeDestroy(SpriteMesh);
        SpriteMesh = Instantiate(mesh);

        SetLayoutTextureDirty();
    }

    public void ModifyMesh(VertexHelper verts)
    {
        if (!isActiveAndEnabled) return;

        // For when pressing play while in prefab mode
        if (!SpriteMesh) SpriteMesh = new Mesh();
        verts.FillMesh(SpriteMesh);

        SetLayoutTextureDirty();
    }

    void SetLayoutTextureDirty()
    {
#if TMP_PRESENT
        if (Graphic is TMPro.TextMeshProUGUI tmp)
        {
            if (tmp.text.Length == 0)
                SpriteMesh = null;
            else
                SpriteMesh = tmp.mesh;
        }
#endif
        SetLayoutDirty();
        SetTextureDirty();
    }
}
}
