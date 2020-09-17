using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EditorColorManager : SerializedMonoBehaviour
{
    public Material _material;
    [Button("Change Color")]
    public void Bleh()
    {
        var b = GetComponentsInChildren<Transform>();
        foreach (Transform VARIABLE in b)
        {
            if (!VARIABLE.TryGetComponent(out Renderer renderer))
            {
                renderer = VARIABLE.gameObject.AddComponent<MeshRenderer>();
                VARIABLE.gameObject.AddComponent<MeshFilter>();
            }

            if (renderer.material == null)
            {
                renderer.material = _material;
            }
        }

    }
}
