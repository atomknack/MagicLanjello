using UnityEngine;
using DoubleEngine;
using DoubleEngine.Atom;
using DoubleEngine.UHelpers;
using MagicLanjello.CellPlaceholder;


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
    private Material[] _unityMaterials;

    public Vector3Int pos;
    private CellPlaceholderStruct _current;
    private CellPlaceholderStruct Current { 
        get => _current; 
        set 
        {
            var prev = _current;
            _current = value;
            UpdateRS();
            UpdateCellMesh();
            UpdateMaterial();
            if (prev.orientation != _current.orientation || 
                prev.material != _current.material ||
                prev.cellMesh != _current.cellMesh)
                StartScaleFlash();
        } 
    }

    private Grid6SidesCached _orientation;
    private Vector3 OrientationToScale() => new Vector3(1, _orientation._invertedY ? -1 : 1, 1);

    public void PositionChanged(Vector3Int newPos)
    {
        pos = newPos;
        transform.position = pos;
    }
    public void PlaceholderChanged(CellPlaceholderStruct to)
    {
        if (Current.Equals(to))
            return;
        Current = to;
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

        pos = Vector3Int.zero;
        Current = CellPlaceholderStruct.DefaultPlaceholder;

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
