using UnityEngine;
using DoubleEngine;
using DoubleEngine.Atom;
using DoubleEngine.UHelpers;
using VectorCore;
using MagicLanjello.CellPlaceholder;
using UKnack.Values;
using System;
using UnityEngine.Events;
using UKnack.Events;
using UKnack.Attributes;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(OscilateScaleFromTo))]
public class CellPlaceholder_React : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private OscilateScaleFromTo _oscilateScale;

    [SerializeField]
    private float _oscilationScale = 1.1f;
    [SerializeField]
    private float _onStartMoveScaleFlash = 1.3f;

    [SerializeField]
    [ValidReference]
    Mesh _meshForEmptyPlaceholder;

    private Vector3Int _cellPos = Vector3Int.zero;
    private CellPlaceholderStruct _current = CellPlaceholderStruct.DefaultPlaceholder;
    private CellPlaceholderStruct Current { 
        get => _current; 
        set
        {
            var prev = _current;
            _current = value;
            if (_initialized == false)
                return;
            UpdateGameObjectToPlaceholder();
            if (prev.orientation != _current.orientation ||
                prev.material != _current.material ||
                prev.cellMesh != _current.cellMesh)
                StartScaleFlash();
        }
    }

    private bool _initialized = false;

    private void UpdateGameObjectToPlaceholder()
    {
        UpdateRS();
        UpdateCellMesh();
        UpdateMaterial();
    }

    private Grid6SidesCached _orientation;
    private Vector3 OrientationToScale() => new Vector3(1, _orientation._invertedY ? -1 : 1, 1);

    public void PositionChanged(Vector3Int newPos)
    {
        _cellPos = newPos;
        transform.position = _cellPos;
    }
    public void PlaceholderChanged(CellPlaceholderStruct to)
    {
        if (Current.Equals(to))
            return;
        Current = to;
    }

    private void Awake()
    {
        if (_meshForEmptyPlaceholder == null)
            throw new System.ArgumentNullException(nameof(_meshForEmptyPlaceholder));
        _oscilateScale = GetComponent<OscilateScaleFromTo>();
        _meshFilter = GetComponent<MeshFilter>();
        _initialized = true;
    }

    void OnEnable()
    {
        UpdateGameObjectToPlaceholder();
        SetOscilationEnabled();

        //GridMaterials.DefaultMaterial;
        //GameEvents.placeholderCellChanged.Publish(meshIndex);
        //GameEvents.placeholderMaterialChanged.Publish(materialIndex);
        //GameEvents.placeholderOrientationChanged.Publish(orientation._orientation);
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
                _meshFilter.sharedMesh = _meshForEmptyPlaceholder;//null;
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
        GetComponent<Renderer>().sharedMaterial = UMaterials.GetUnityMaterial(Current.material);
    }

    private void UpdateRS()
    {
        _orientation = Grid6SidesCached.FromRotationAndScale(ScaleInversionPerpendicularRotation3.FromInt(Current.orientation));
        transform.rotation = _orientation._rotation.ToQuaternion();
        transform.localScale = OrientationToScale();
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
