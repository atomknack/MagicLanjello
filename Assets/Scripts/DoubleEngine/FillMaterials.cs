using DoubleEngine.Atom;
using DoubleEngine.UHelpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillMaterials : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"before FillMaterials there is {DEMaterials.Count} materials");
        UMaterials.GetUnityMaterial(0);
        Debug.Log($"after FillMaterials there is {DEMaterials.Count} materials");
    }
}
