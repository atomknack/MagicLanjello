using DoubleEngine;
using DoubleEngine.Atom;
using DoubleEngine.UHelpers;
using System;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(OscilateScaleFromTo))]
public class CellPlaceholder_React : MonoBehaviour
{
    [System.Serializable]
    public struct CellPlaceholderStruct : IEquatable<CellPlaceholderStruct>
    {
        public Vector3Int pos;
        public int orientation;
        public short cellMesh;
        public byte material;

        public bool Equals(CellPlaceholderStruct other) =>
            pos == other.pos &&
            orientation == other.orientation &&
            cellMesh == other.cellMesh &&
            material == other.material;
    }

    private MeshFilter _meshFilter;
    private OscilateScaleFromTo _oscilateScale;

    [SerializeField]
    private float _oscilationScale = 1.1f;
    [SerializeField]
    private float _onStartMoveScaleFlash = 1.3f;

    [SerializeField]
    private Material[] _unityMaterials;

    private CellPlaceholderStruct Current { get; set; }

    private Grid6SidesCached _orientation;
    private Vector3 OrientationToScale() => new Vector3(1, _orientation._invertedY ? -1 : 1, 1);

    public void PlaceholderChanged(CellPlaceholderStruct to)
    {
        if (Current.Equals(to))
            return;

        Current = to;
        UpdatePlaceholder();
    }
    void OnEnable()
    {
        _oscilateScale = GetComponent<OscilateScaleFromTo>();
        //if (gridMesh == null)
        //    gridMesh = FindObjectOfType<AbstractGridMeshGenerator>();
        _meshFilter = GetComponent<MeshFilter>();
        //meshFilterDefaultCube = meshFilter.sharedMesh;


        //meshIndex = 1;
        //materialIndex = DEMaterials.DefaultMaterial.id;
        //orientation = Grid6SidesCached.Default;

        Current = new CellPlaceholderStruct { pos = Vector3Int.zero, orientation = 0, cellMesh = 1, material = DEMaterials.DefaultMaterial.id };
        UpdatePlaceholder();
        //GridMaterials.DefaultMaterial;
        //GameEvents.placeholderCellChanged.Publish(meshIndex);
        //GameEvents.placeholderMaterialChanged.Publish(materialIndex);
        //GameEvents.placeholderOrientationChanged.Publish(orientation._orientation);
    }

    private void UpdatePlaceholder()
    {
        UpdateTRS();
        UpdateCellMesh();
        UpdateMaterial();
        StartScaleFlash();
    }





    private void UpdateCellMesh()
    {
        short index = Current.cellMesh;
        if (index < 0 || index >= ThreeDimensionalCellMeshes.GetCount())
            throw new System.ArgumentOutOfRangeException($"index {index}");
        ChangeMeshIndex(index);

        void ChangeMeshIndex(short newIndex)
        {
            if (newIndex == 0)
                _meshFilter.sharedMesh = null;
            else
                _meshFilter.sharedMesh = UThreeDimensionalCellMeshes.GetUnityMesh((short)newIndex);
        }
    }
    private void StartScaleFlash()
    {
        var startScale = OrientationToScale() * _onStartMoveScaleFlash;
        var endScale = OrientationToScale();
        SetOscilationDisabled();
        CoroutineAnimationsSingleton.ChangeScaleFromToOverTime(gameObject, startScale, endScale, 0.1f, SetOscilationEnabled);
    }
    private void UpdateMaterial()
    {
        GetComponent<Renderer>().sharedMaterial = _unityMaterials[Current.material];//DEMaterials.GetUnityMaterial(materialIndex);
    }

    private void UpdateTRS()
    {
        _orientation = Grid6SidesCached.FromRotationAndScale(ScaleInversionPerpendicularRotation3.FromInt(Current.orientation));
        transform.rotation = _orientation._rotation.ToQuaternion();
        transform.localScale = OrientationToScale();
        transform.position = Current.pos;
    }
    private void SetOscilationEnabled()
    {
        var fromScale = OrientationToScale();
        _oscilateScale.fromScale = fromScale;
        _oscilateScale.toScale = fromScale * _oscilationScale;
        _oscilateScale.enabled = true;
    }
    private void SetOscilationDisabled()
    {
        _oscilateScale.enabled = false;
        transform.localScale = OrientationToScale();
    }

}
